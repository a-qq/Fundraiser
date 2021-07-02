using FluentAssertions;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SchoolManagement.Domain.SchoolAggregate.Schools.Events;
using SharedKernel.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using SK = SharedKernel.Domain.Constants;
using Xunit;

namespace SchoolManagement.UnitTests.SchoolAggregateTests.SchoolsTests
{
    public class SchoolTests
    {
        [Fact]
        public void Constructor_Creates_Valid_School()
        {
            var name = Name.Create("II LO Rzeszów im. J. Korczaka").Value;
            var yearsOfEducation = YearsOfEducation.Create(3).Value;
            var firstName = FirstName.Create("Jan").Value;
            var lastName = LastName.Create("Nowak").Value;
            var email = Email.Create("jan_nowak@gmail.com").Value;
            var gender = Gender.Create("male").Value;

            var sut = new School(name, yearsOfEducation, firstName, lastName, email, gender);

            sut.Name.Should().Be(name);
            sut.YearsOfEducation.Should().Be(yearsOfEducation);
            sut.GroupMembersLimit.Should().BeNull();
            sut.Description.Should().BeNull();
            sut.LogoId.Should().BeNull();
            sut.Groups.Should().BeEmpty();
            sut.Members.Should().HaveCount(1);
            sut.Members[0].FirstName.Should().Be(firstName);
            sut.Members[0].LastName.Should().Be(lastName);
            sut.Members[0].Email.Should().Be(email);
            sut.Members[0].Gender.Should().Be(gender);
            sut.Members[0].IsActive.Should().BeFalse();
            sut.Members[0].IsArchived.Should().BeFalse();
            sut.Members[0].Role.Should().Be(Role.Headmaster);
            sut.Members[0].School.Should().Be(sut);
            sut.Members[0].Group.Should().BeNull();
            sut.DomainEvents.Should().HaveCount(1);
            sut.DomainEvents[0].Should().BeOfType<MemberEnrolledDomainEvent>();
            var sutEvent = sut.DomainEvents[0].As<MemberEnrolledDomainEvent>();
            sutEvent.MemberId.Should().Be(sut.Members[0].Id);
            sutEvent.SchoolId.Should().Be(sut.Id);
        }

        public class EditTests
        {
            [Fact]
            public void Editing_school_name_description_and_group_member_limit()
            {
                var name = Name.Create("Test name").Value;
                var description = Description.Create("Example description").Value;
                var gml = GroupMembersLimit.Create(30).Value;
                var sut = CreateSchool();

                var result = sut.Edit(name, description, gml);

                result.IsSuccess.Should().BeTrue();
                sut.Name.Should().Be(name);
                sut.Description.Should().Be(description);
                sut.GroupMembersLimit.Should().Be(gml);
            }

            [Fact]
            public void Decreasing_group_member_limit_to_lower_than_member_count_of_any_group_is_invalid()
            {
                var description = Description.Create(null).Value;
                var sut = CreateSchool();
                sut.EditInfo(description, GroupMembersLimit.Create(2).Value);
                var groups = sut.AddGroups(2, 2, 'b');
                var members = sut.EnrollMembersAs(Role.Student, 4);
                sut.AssignStudentToGroup(members[0].Id, groups[0].Code);
                sut.AssignStudentToGroup(members[1].Id, groups[0].Code);
                sut.AssignStudentToGroup(members[2].Id, groups[1].Code);
                sut.AssignStudentToGroup(members[3].Id, groups[1].Code);
                var errors = new[]
                {
                    $"Group '2a' (Id: '{groups[0].Id}') has more then '1' student(s)! (Currently: '2')",
                    $"Group '2b' (Id: '{groups[1].Id}') has more then '1' student(s)! (Currently: '2')"
                };

                var result = sut.EditInfo(description, GroupMembersLimit.Create(1).Value);

                result.IsFailure.Should().BeTrue();
                result.Error.Errors.Should().HaveCount(2);
                result.Error.Errors.Should().BeEquivalentTo(errors);
            }

            [Fact]
            public void Edit_logo_generates_new_logo_id()
            {
                var sut = CreateSchool();
                sut.EditLogo();
                var oldLogoId = sut.LogoId;
                sut.EditLogo();

                sut.LogoId.Should().NotBeNullOrWhiteSpace();
                sut.LogoId.Should().NotBe(oldLogoId);
            }
        }

        public class MemberEnrollmentTests
        {

            [Theory]
            [MemberData(nameof(RolesData))]
            public void Candidate_is_enrolled(Role role)
            {
                var sut = CreateSchool();
                sut.DivestHeadmaster();
                sut.ClearEvents();

                var firstName = FirstName.Create("Jan").Value;
                var lastName = LastName.Create("Kowalski").Value;
                var email = Email.Create("test1@o2.pl").Value;
                var gender = Gender.Male;

                var result = sut.EnrollCandidate(FirstName.Create("Jan").Value, LastName.Create("Kowalski").Value,
                    Email.Create("test1@o2.pl").Value, role, Gender.Male);

                result.IsSuccess.Should().BeTrue();
                sut.Members[1].Should().Be(result.Value);
                sut.Members.Should().HaveCount(2);
                sut.Members[1].FirstName.Should().Be(firstName);
                sut.Members[1].LastName.Should().Be(lastName);
                sut.Members[1].Email.Should().Be(email);
                sut.Members[1].Gender.Should().Be(gender);
                sut.Members[1].IsActive.Should().BeFalse();
                sut.Members[1].IsArchived.Should().BeFalse();
                sut.Members[1].Role.Should().Be(role);
                sut.Members[1].School.Should().Be(sut);
                sut.Members[1].Group.Should().BeNull();
                sut.DomainEvents.Should().HaveCount(1);
                sut.DomainEvents[0].Should().BeOfType<MemberEnrolledDomainEvent>();
                var sutEvent = sut.DomainEvents[0].As<MemberEnrolledDomainEvent>();
                sutEvent.SchoolId.Should().Be(sut.Id);
                sutEvent.MemberId.Should().Be(result.Value.Id);
            }

            public static List<object[]> RolesData()
            {
                return new List<object[]>
                {
                    new object[] {Role.Teacher},
                    new object[] {Role.Headmaster},
                    new object[] {Role.Student}
                };
            }

            [Fact]
            public void Enrolling_headmaster_fails_when_school_already_has_one()
            {
                var sut = CreateSchool();

                var firstName = FirstName.Create("Jan").Value;
                var lastName = LastName.Create("Kowalski").Value;
                var email = Email.Create("test1@o2.pl").Value;

                var result = sut.EnrollCandidate(firstName, lastName, email, Role.Headmaster, Gender.Male);
                result.IsFailure.Should().BeTrue();
                result.Error.Errors.Should()
                    .BeEquivalentTo($"School already have a headmaster: 'jan_nowak@gmail.com' (Id: '{sut.Members[0].Id}')");
            }
        }

        public class MemberExpulsionTests
        {
            [Fact]
            public void Headmaster_cannot_be_expelled()
            {
                var sut = CreateSchool();
                var headmasterId = sut.Members[0].Id;
                var result = sut.ExpelMember(headmasterId);

                result.IsFailure.Should().BeTrue();
                result.Error.Errors.Should()
                    .BeEquivalentTo($"Headmaster 'jan_nowak@gmail.com' (Id: '{headmasterId}' cannot be expelled!");
            }

            [Theory]
            [MemberData(nameof(ExpulsionData))]
            public void Member_is_expelled(Role role, bool isInGroup, bool isTreasurer, bool isFormTutor, bool isInArchivedGroup)
            {
                var sut = CreateSchool();
                var member = sut.EnrollMembersAs(role, 1)[0];
                var group = sut.AddGroups(3, 3, 'a')[0];
                if (isInGroup)
                {
                    if (isFormTutor)
                        sut.PromoteFormTutor(group.Id, member.Id);
                    else
                    {
                        sut.AssignStudentToGroup(member.Id, group.Code);
                        if (isTreasurer)
                            sut.PromoteTreasurer(group.Id, member.Id);
                    }

                    if (isInArchivedGroup)
                    {
                        sut.MarkMemberAsActive(member.Id);
                        sut.Graduate();
                    }
                }
                sut.ClearEvents();

                var result = sut.ExpelMember(member.Id);

                result.IsSuccess.Should().BeTrue();
                sut.Members.Should().HaveCount(1);
                sut.Members.Should().NotContain(member);
                sut.DomainEvents.Should().HaveCount(1);
                sut.DomainEvents[0].Should().BeOfType<MemberExpelledDomainEvent>();
                sut.DomainEvents[0].As<MemberExpelledDomainEvent>().MemberId.Should().Be(member.Id);
                sut.Groups[0].FormTutor.Should().BeNull();
                sut.Groups[0].Treasurer.Should().BeNull();
                sut.Groups[0].Students.Should().BeEmpty();
            }

            public static List<object[]> ExpulsionData()
            {
                return new List<object[]>
                {
                    new object[] { Role.Student, false, false, false, false },
                    new object[] { Role.Student, true, false, false, false },
                    new object[] { Role.Student, true, true, false, false },
                    new object[] { Role.Student, true, true, false, true },
                    new object[] { Role.Teacher, false, false, false, false },
                    new object[] { Role.Teacher, true, false, true, false },
                    new object[] { Role.Teacher, true, false, true, true }
                };
            }
        }

        public class MemberArchivisationTests
        {
            [Fact]
            public void Archived_member_cannot_be_archived()
            {
                var sut = CreateSchool();
                var member = sut.EnrollMembersAs(Role.Teacher, 1)[0];
                sut.MarkMemberAsActive(member.Id);
                sut.ArchiveMember(member.Id);
                sut.ClearEvents();

                var result = sut.ArchiveMember(member.Id);

                result.IsFailure.Should().BeTrue();
                result.Error.Errors.Should().HaveCount(1);
                result.Error.Errors.Should().BeEquivalentTo($"Teacher 'test@o2.pl' (Id: '{member.Id}') is already archived!");
            }

            [Fact]
            public void Headmaster_cannot_be_archived()
            {
                var sut = CreateSchool();
                var headmasterId = sut.Members[0].Id;
                sut.MarkMemberAsActive(headmasterId);
                sut.ClearEvents();

                var result = sut.ArchiveMember(headmasterId);

                result.IsFailure.Should().BeTrue();
                result.Error.Errors.Should().HaveCount(1);
                result.Error.Errors.Should().BeEquivalentTo($"Headmaster 'jan_nowak@gmail.com' (Id: '{headmasterId}') cannot be archived!");
            }

            [Fact]
            public void Inactive_member_cannot_be_archived()
            {
                var sut = CreateSchool();
                var member = sut.EnrollMembersAs(Role.Student, 1)[0];
                sut.ClearEvents();

                var result = sut.ArchiveMember(member.Id);

                result.IsFailure.Should().BeTrue();
                result.Error.Errors.Should().HaveCount(1);
                result.Error.Errors.Should().BeEquivalentTo($"Cannot archive not active member 'test@o2.pl' (Id: '{member.Id}')!");
            }

            [Theory]
            [MemberData(nameof(ArchivisationData))]
            public void Member_is_archived(Role role, bool isInGroup, bool isTreasurer, bool isFormTutor)
            {
                var sut = CreateSchool();
                var member = sut.EnrollMembersAs(role, 1)[0];
                var group = sut.AddGroups(1, 1, 'a')[0];
                sut.MarkMemberAsActive(member.Id);
                if (isInGroup)
                {
                    if (isFormTutor)
                        sut.PromoteFormTutor(group.Id, member.Id);
                    else
                    {
                        sut.AssignStudentToGroup(member.Id, group.Code);
                        if (isTreasurer)
                            sut.PromoteTreasurer(group.Id, member.Id);
                    }
                }
                sut.ClearEvents();

                var result = sut.ArchiveMember(member.Id);

                result.IsSuccess.Should().BeTrue();
                sut.Members.Should().HaveCount(2);
                sut.Members.Should().Contain(member);
                sut.DomainEvents.Should().HaveCount(1);
                sut.DomainEvents[0].Should().BeOfType<MemberArchivedDomainEvent>();
                var sutEvent = sut.DomainEvents[0].As<MemberArchivedDomainEvent>();
                sutEvent.MemberId.Should().Be(member.Id);
                if (isTreasurer)
                    sutEvent.GroupRole.Should().Be(SK.GroupRoles.Treasurer);
                else if (isFormTutor)
                    sutEvent.GroupRole.Should().Be(SK.GroupRoles.FormTutor);
                else sutEvent.GroupRole.Should().BeNull();
                sut.Groups[0].FormTutor.Should().BeNull();
                sut.Groups[0].Treasurer.Should().BeNull();
                sut.Groups[0].Students.Should().BeEmpty();
            }

            public static List<object[]> ArchivisationData()
            {
                return new List<object[]>
                {
                    new object[] { Role.Student, false, false, false },
                    new object[] { Role.Student, true, false, false },
                    new object[] { Role.Student, true, true, false, },
                    new object[] { Role.Teacher, false, false, false },
                    new object[] { Role.Teacher, true, false, true },
                };
            }
        }

        public class MemberReactivationTests
        {
            [Fact]
            public void Cannot_reactivate_not_archived_member()
            {
                var sut = CreateSchool();
                var member = sut.EnrollMembersAs(Role.Student, 1)[0];
                sut.ClearEvents();

                var result = sut.RestoreMember(member.Id);
                result.IsFailure.Should().BeTrue();
                result.Error.Errors.Should().HaveCount(1);
                result.Error.Errors.Should().BeEquivalentTo($"Member 'test@o2.pl' (Id: '{member.Id}') is not archived!");
            }

            [Theory]
            [MemberData(nameof(ReactivationData))]
            public void Member_is_restored(Role role, bool isInArchivedGroup, bool isTreasurer, bool isFormTutor)
            {
                var sut = CreateSchool();
                var member = sut.EnrollMembersAs(role, 1)[0];
                var group = sut.AddGroups(3, 3, 'a')[0];
                sut.MarkMemberAsActive(member.Id);
                if (isInArchivedGroup)
                {
                    if (isFormTutor)
                    {
                        sut.PromoteFormTutor(group.Id, member.Id);
                        sut.Graduate();
                        sut.ArchiveMember(member.Id);
                    }
                    else
                    {
                        sut.AssignStudentToGroup(member.Id, group.Code);
                        if (isTreasurer)
                            sut.PromoteTreasurer(group.Id, member.Id);
                        sut.Graduate();
                    }
                }
                else sut.ArchiveMember(member.Id);

                sut.ClearEvents();

                var result = sut.RestoreMember(member.Id);

                result.IsSuccess.Should().BeTrue();
                member.IsArchived.Should().BeFalse();
                sut.Members.Should().HaveCount(2);
                sut.Members.Should().Contain(member);
                sut.DomainEvents.Should().HaveCount(1);
                sut.DomainEvents[0].Should().BeOfType<MemberRestoredDomainEvent>()
                    .Which.MemberId.Should().Be(member.Id);
                sut.Groups[0].Treasurer.Should().BeNull();
                if (isFormTutor)
                    sut.Groups[0].FormTutor.Should().Be(member);
                else sut.Groups[0].FormTutor.Should().BeNull();
                sut.Groups[0].Students.Should().BeEmpty();
            }

            public static List<object[]> ReactivationData()
            {
                return new List<object[]>
                {
                    new object[] { Role.Student, false, false, false },
                    new object[] { Role.Student, true, false, false },
                    new object[] { Role.Student, true, true, false, },
                    new object[] { Role.Teacher, false, false, false },
                    new object[] { Role.Teacher, true, false, true },
                };
            }
        }

        public class GroupCreationTests
        {
            [Fact]
            public void Group_number_cannot_be_greater_than_years_of_education()
            {
                var sut = CreateSchool();
                var number = Number.Create(4).Value;
                var sign = Sign.Create("b").Value;

                var result = sut.CreateGroup(number, sign);

                result.IsFailure.Should().BeTrue();
                result.Error.Errors.Should().HaveCount(1);
                result.Error.Errors.Should().BeEquivalentTo(
                    "Number ('4') cannot be greater then school's years of education ('3')!");
            }

            [Fact]
            public void Group_code_must_be_unique_and_case_insensitive()
            {
                var sut = CreateSchool();
                var number = Number.Create(1).Value;
                var signLowerCase = Sign.Create("b").Value;
                var signUpperCase = Sign.Create("B").Value;
                sut.CreateGroup(number, signLowerCase);

                var result = sut.CreateGroup(number, signUpperCase);

                result.IsFailure.Should().BeTrue();
                result.Error.Errors.Should().BeEquivalentTo("Group with code 1B already exist!");
            }

            [Fact]
            public void Group_is_added_to_school_when_archived_equivalent_exists()
            {
                var sut = CreateSchool();
                var number = Number.Create(3).Value;
                var sign = Sign.Create("b").Value;
                sut.CreateGroup(number, sign);
                sut.Graduate();
                sut.ClearEvents();

                var result = sut.CreateGroup(number, sign);

                result.IsSuccess.Should().BeTrue();
                sut.Groups.Should().HaveCount(2);
                sut.DomainEvents.Should().BeEmpty();
                sut.Groups[1].Should().Be(result.Value);
                sut.Groups[1].Number.Should().Be(number);
                sut.Groups[1].Sign.Should().Be(sign);
                sut.Groups[1].Code.Value.Should().Be("3b");
                sut.Groups[1].FormTutor.Should().BeNull();
                sut.Groups[1].School.Should().Be(sut);
                sut.Groups[1].Treasurer.Should().BeNull();
                sut.Groups[1].IsArchived.Should().BeFalse();
                sut.Groups[1].Students.Should().BeEmpty();
            }
        }

        public class StudentAssignmentTests
        {
            [Fact]
            public void Student_already_in_group()
            {
                var sut = CreateSchool();
                var groups = sut.AddGroups(1, 1, 'b');
                var member = sut.EnrollMembersAs(Role.Student, 1)[0]; 
                sut.AssignStudentToGroup(member.Id, groups[0].Code);

                var result = sut.AssignStudentToGroup(member.Id, groups[1].Code);

                result.IsFailure.Should().BeTrue();
                result.Error.Errors.Should().HaveCount(1);
                result.Error.Errors.Should().BeEquivalentTo(
                    $"'test@o2.pl' (Id: '{member.Id}') is already member of group '1a'!");
            }

            [Fact]
            public void Group_cannot_cross_member_limit()
            {
                var sut = CreateSchool();
                var group = sut.AddGroups(1, 1, 'a')[0];
                var members = sut.EnrollMembersAs(Role.Student, 2);
                sut.AssignStudentToGroup(members[0].Id, group.Code);
                sut.EditInfo(Description.Create(null).Value, GroupMembersLimit.Create(1).Value);

                var result = sut.AssignStudentToGroup(members[1].Id, group.Code);

                result.IsFailure.Should().BeTrue();
                result.Error.Errors.Should().HaveCount(1);
                result.Error.Errors.Should().BeEquivalentTo(
                    $"Group '1a' (Id: '{group.Id}') is full (max student count: '1')!");
            }

            [Fact]
            public void Only_student_can_be_assigned_to_group()
            {
                var sut = CreateSchool();
                var group = sut.AddGroups(1, 1, 'a')[0];
                var member = sut.EnrollMembersAs(Role.Teacher, 1)[0];

                var exception = Record.Exception(() => sut.AssignStudentToGroup(member.Id, group.Code));

                Assert.IsType<InvalidOperationException>(exception);
            }

            [Fact]
            public void Student_cannot_be_assigned_to_archived_group()
            {
                var sut = CreateSchool();
                var group = sut.AddGroups(3, 3, 'a')[0];
                var member = sut.EnrollMembersAs(Role.Student, 1)[0];
                sut.Graduate();

                var exception = Record.Exception(() => sut.AssignStudentToGroup(member.Id, group.Code));

                Assert.IsType<InvalidOperationException>(exception);
            }

            [Fact]
            public void Archived_student_cannot_be_assigned_to_group()
            {
                var sut = CreateSchool();
                var group = sut.AddGroups(1, 1, 'a')[0];
                var member = sut.EnrollMembersAs(Role.Student, 1)[0];
                sut.MarkMemberAsActive(member.Id);
                sut.ArchiveMember(member.Id);

                var exception = Record.Exception(() => sut.AssignStudentToGroup(member.Id, group.Code));

                Assert.IsType<InvalidOperationException>(exception);
            }

            [Fact]
            public void Student_is_assigned_to_group()
            {
                var sut = CreateSchool();
                var group = sut.AddGroups(1, 1, 'a')[0];
                var member = sut.EnrollMembersAs(Role.Student, 1)[0];
                sut.EditInfo(Description.Create(null).Value, GroupMembersLimit.Create(10).Value);
                sut.ClearEvents();

                var result = sut.AssignStudentToGroup(member.Id, group.Code);

                result.IsSuccess.Should().BeTrue();
                sut.DomainEvents.Should().HaveCount(1);
                sut.DomainEvents[0].Should().BeOfType<StudentAssignedDomainEvent>();
                var sutEvent = sut.DomainEvents[0].As<StudentAssignedDomainEvent>();
                sutEvent.GroupId.Should().Be(group.Id);
                sutEvent.StudentId.Should().Be(member.Id);
                sut.Groups[0].Students.Should().HaveCount(1);
                sut.Groups[0].Students[0].Should().Be(member);
                sut.Members[1].Group.Should().Be(group);
            }
        }

        public class FormTutorPromotionTests
        {
            [Fact]
            public void Group_already_has_a_form_tutor_and_teacher_is_a_form_tutor_of_other_group()
            {
                var sut = CreateSchool();
                var groups = sut.AddGroups(1, 1, 'b');
                var members = sut.EnrollMembersAs(Role.Teacher, 2);
                sut.PromoteFormTutor(groups[0].Id, members[0].Id);
                sut.PromoteFormTutor(groups[1].Id, members[1].Id);

                var result = sut.PromoteFormTutor(groups[0].Id, members[1].Id);

                result.IsFailure.Should().BeTrue();
                result.Error.Errors.Should().HaveCount(2);
                result.Error.Errors.Should().BeEquivalentTo(
                    $"Group '1a'(Id: '{groups[0].Id}') already has a form tutor!",
                    $"'test@o2.pl'(Id: '{members[1].Id}') is already form tutor of group '1b'!");
            }

            [Fact]
            public void Member_must_be_a_teacher_to_be_promoted()
            {
                var sut = CreateSchool();
                var group = sut.AddGroups(1, 1, 'a')[0];
                var member = sut.EnrollMembersAs(Role.Student, 1)[0];

                var exception = Record.Exception(() => sut.PromoteFormTutor(group.Id, member.Id));

                Assert.IsType<InvalidOperationException>(exception);
            }

            [Fact]
            public void Teacher_cannot_be_archived_to_be_promoted()
            {
                var sut = CreateSchool();
                var group = sut.AddGroups(1, 1, 'a')[0];
                var member = sut.EnrollMembersAs(Role.Teacher, 1)[0];
                sut.MarkMemberAsActive(member.Id);
                sut.ArchiveMember(member.Id);

                var exception = Record.Exception(() => sut.PromoteFormTutor(group.Id, member.Id));

                Assert.IsType<InvalidOperationException>(exception);
            }

            [Fact]
            public void Archived_group_cannot_be_a_target_of_promotion()
            {
                var sut = CreateSchool();
                var group = sut.AddGroups(3, 3, 'a')[0];
                var member = sut.EnrollMembersAs(Role.Teacher, 1)[0];
                sut.Graduate();

                var exception = Record.Exception(() => sut.PromoteFormTutor(group.Id, member.Id));

                Assert.IsType<InvalidOperationException>(exception);
            }

            [Fact]
            public void Teacher_is_promoted_to_form_tutor()
            {
                var sut = CreateSchool();
                var group = sut.AddGroups(1, 1, 'a')[0];
                var member = sut.EnrollMembersAs(Role.Teacher, 1)[0];
                sut.ClearEvents();
                
                var result = sut.PromoteFormTutor(group.Id, member.Id);

                result.IsSuccess.Should().BeTrue();
                sut.Groups[0].FormTutor.Should().Be(member);
                sut.DomainEvents.Should().HaveCount(1);
                sut.DomainEvents[0].Should().BeOfType<FormTutorAssignedDomainEvent>();
                var sutEvent = sut.DomainEvents[0].As<FormTutorAssignedDomainEvent>();
                sutEvent.TeacherId.Should().Be(member.Id);
                sutEvent.GroupId.Should().Be(group.Id);
            }

            [Fact]
            public void Teacher_is_promoted_to_form_tutor_after_his_previous_group_graduated()
            {
                var sut = CreateSchool();
                var groups = sut.AddGroups(2, 3, 'a');
                var member = sut.EnrollMembersAs(Role.Teacher, 1)[0];
                sut.PromoteFormTutor(groups[1].Id, member.Id);
                sut.Graduate();
                sut.ClearEvents();

                var result = sut.PromoteFormTutor(groups[0].Id, member.Id);

                result.IsSuccess.Should().BeTrue();
                sut.Groups[0].FormTutor.Should().Be(member);
                sut.Groups[1].FormTutor.Should().Be(member);
                sut.DomainEvents.Should().HaveCount(1);
                sut.DomainEvents[0].Should().BeAssignableTo<FormTutorAssignedDomainEvent>();
                var sutEvent = sut.DomainEvents[0].As<FormTutorAssignedDomainEvent>();
                sutEvent.TeacherId.Should().Be(member.Id);
                sutEvent.GroupId.Should().Be(groups[0].Id);
            }
        }

        public class FormTutorDivestmentTests
        {
            [Fact]
            public void Form_tutor_of_archived_group_cannot_be_divested()
            {
                var sut = CreateSchool();
                var group = sut.AddGroups(3, 3, 'a')[0];
                var member = sut.EnrollMembersAs(Role.Teacher, 1)[0];
                sut.PromoteFormTutor(group.Id, member.Id);
                sut.Graduate();

                var exception = Record.Exception(() => sut.DivestFormTutorFromGroup(group.Id));

                Assert.IsType<InvalidOperationException>(exception);
            }

            [Fact]
            public void Divest_form_tutor_throws_when_groups_does_not_have_one()
            {
                var sut = CreateSchool();
                var group = sut.AddGroups(3, 3, 'a')[0];

                var exception = Record.Exception(() => sut.DivestFormTutorFromGroup(group.Id));

                Assert.IsType<InvalidOperationException>(exception);
            }

            [Fact]
            public void Form_tutor_is_divested()
            {
                var sut = CreateSchool();
                var group = sut.AddGroups(3, 3, 'a')[0];
                var member = sut.EnrollMembersAs(Role.Teacher, 1)[0];
                sut.PromoteFormTutor(group.Id, member.Id);
                sut.ClearEvents();

                sut.DivestFormTutorFromGroup(group.Id);

                sut.Groups[0].FormTutor.Should().BeNull();
                sut.DomainEvents.Should().HaveCount(1);
                sut.DomainEvents[0].Should().BeOfType<FormTutorDivestedDomainEvent>();
                sut.DomainEvents[0].As<FormTutorDivestedDomainEvent>().FormTutorId.Should().Be(member.Id);
            }
        }

        public class TreasurerPromotionTests
        {
            [Fact]
            public void Member_must_be_student_of_targeted_group()
            {
                var sut = CreateSchool();
                var group = sut.AddGroups(1, 1, 'a')[0];
                var member = sut.EnrollMembersAs(Role.Student, 1)[0];

                var exception = Record.Exception(() => sut.PromoteTreasurer(group.Id, member.Id));

                Assert.IsType<InvalidOperationException>(exception);
            }

            [Fact]
            public void Group_already_has_the_treasurer()
            {
                var sut = CreateSchool();
                var group = sut.AddGroups(1, 1, 'a')[0];
                var members = sut.EnrollMembersAs(Role.Student, 2);
                sut.AssignStudentToGroup(members[0].Id, group.Code);
                sut.AssignStudentToGroup(members[1].Id, group.Code);
                sut.PromoteTreasurer(group.Id, members[0].Id);
                sut.ClearEvents();
                
                var result = sut.PromoteTreasurer(group.Id, members[1].Id);

                result.IsFailure.Should().BeTrue();
                result.Error.Errors.Should().HaveCount(1);
                result.Error.Errors.Should().BeEquivalentTo
                    ($"Group '1a' (Id: '{group.Id}') already has a treasurer!");
            }

            [Fact]
            public void Archived_group_cannot_be_a_target_of_promotion()
            {
                var sut = CreateSchool();
                var group = sut.AddGroups(3, 3, 'a')[0];
                var member = sut.EnrollMembersAs(Role.Student, 1)[0];
                sut.AssignStudentToGroup(member.Id, group.Code);
                sut.MarkMemberAsActive(member.Id);
                sut.Graduate();

                var exception = Record.Exception(() => sut.PromoteTreasurer(group.Id, member.Id));

                Assert.IsType<InvalidOperationException>(exception);
            }

            [Fact]
            public void Student_is_promoted_to_treasurer()
            {
                var sut = CreateSchool();
                var group = sut.AddGroups(3, 3, 'a')[0];
                var member = sut.EnrollMembersAs(Role.Student, 1)[0];
                sut.AssignStudentToGroup(member.Id, group.Code);
                sut.ClearEvents();

                var result = sut.PromoteTreasurer(group.Id, member.Id);

                result.IsSuccess.Should().BeTrue();
                sut.Groups[0].Treasurer.Should().Be(member);
                sut.DomainEvents.Should().HaveCount(1);
                sut.DomainEvents[0].Should().BeOfType<TreasurerPromotedDomainEvent>();
                sut.DomainEvents[0].As<TreasurerPromotedDomainEvent>().StudentId.Should().Be(member.Id);
            }
        }

        public class TreasurerDivestmentTests
        {
            [Fact]
            public void Treasurer_of_archived_group_cannot_be_divested()
            {
                var sut = CreateSchool();
                var group = sut.AddGroups(3, 3, 'a')[0];
                var member = sut.EnrollMembersAs(Role.Student, 1)[0];
                sut.AssignStudentToGroup(member.Id, group.Code);
                sut.PromoteTreasurer(group.Id, member.Id);
                sut.MarkMemberAsActive(member.Id);
                sut.Graduate();

                var exception = Record.Exception(() => sut.DivestTreasurerFromGroup(group.Id));

                Assert.IsType<InvalidOperationException>(exception);
            }

            [Fact]
            public void Divest_treasurer_throws_when_groups_does_not_have_one()
            {
                var sut = CreateSchool();
                var group = sut.AddGroups(3, 3, 'a')[0];

                var exception = Record.Exception(() => sut.DivestTreasurerFromGroup(group.Id));

                Assert.IsType<InvalidOperationException>(exception);
            }

            [Fact]
            public void Treasurer_is_divested()
            {
                var sut = CreateSchool();
                var group = sut.AddGroups(3, 3, 'a')[0];
                var member = sut.EnrollMembersAs(Role.Student, 1)[0];
                sut.AssignStudentToGroup(member.Id, group.Code);
                sut.PromoteTreasurer(group.Id, member.Id);
                sut.ClearEvents();

                sut.DivestTreasurerFromGroup(group.Id);

                sut.Groups[0].Treasurer.Should().BeNull();
                sut.DomainEvents.Should().HaveCount(1);
                sut.DomainEvents[0].Should().BeOfType<TreasurerDivestedDomainEvent>();
                sut.DomainEvents[0].As<TreasurerDivestedDomainEvent>().TreasurerId.Should().Be(member.Id);
            }
        }

        public class StudentDisenrollmentTests
        {
            [Fact]
            public void Archived_group_cannot_be_a_target_of_disenrollment()
            {
                var sut = CreateSchool();
                var group = sut.AddGroups(3, 3, 'a')[0];
                var member = sut.EnrollMembersAs(Role.Student, 1)[0];
                sut.AssignStudentToGroup(member.Id, group.Code);
                sut.MarkMemberAsActive(member.Id);
                sut.Graduate();

                var exception = Record.Exception(() => sut.PromoteFormTutor(group.Id, member.Id));

                Assert.IsType<InvalidOperationException>(exception);
            }

            [Fact]
            public void Treasurer_is_disenrolled_from_group()
            {
                var sut = CreateSchool();
                var group = sut.AddGroups(1, 1, 'a')[0];
                var member = sut.EnrollMembersAs(Role.Student, 1)[0];
                sut.AssignStudentToGroup(member.Id, group.Code);
                sut.PromoteTreasurer(group.Id, member.Id);
                sut.ClearEvents();

                sut.DisenrollStudentFromGroup(group.Id, member.Id);

                sut.Groups[0].Treasurer.Should().BeNull();
                sut.Groups[0].Students.Should().BeEmpty();
                sut.DomainEvents.Should().HaveCount(1);
                sut.DomainEvents[0].Should().BeOfType<StudentDisenrolledDomainEvent>();
                var sutEvent = sut.DomainEvents[0].As<StudentDisenrolledDomainEvent>();
                sutEvent.StudentId.Should().Be(member.Id);
                sutEvent.RemovedRole.Should().Be(SK.GroupRoles.Treasurer);
            }

            [Fact]
            public void Student_is_disenrolled_from_group()
            {
                var sut = CreateSchool();
                var group = sut.AddGroups(1, 1, 'a')[0];
                var member = sut.EnrollMembersAs(Role.Student, 1)[0];
                sut.AssignStudentToGroup(member.Id, group.Code);
                sut.ClearEvents();

                sut.DisenrollStudentFromGroup(group.Id, member.Id);

                sut.Groups[0].Treasurer.Should().BeNull();
                sut.Groups[0].Students.Should().BeEmpty();
                sut.Members[1].Group.Should().BeNull();
                sut.DomainEvents.Should().HaveCount(1);
                sut.DomainEvents[0].Should().BeOfType<StudentDisenrolledDomainEvent>();
                var sutEvent = sut.DomainEvents[0].As<StudentDisenrolledDomainEvent>();
                sutEvent.StudentId.Should().Be(member.Id);
                sutEvent.RemovedRole.Should().BeNull();
            }
        }

        public class HeadmasterDivestmentTests
        {
            [Fact]
            public void Headmaster_is_divested()
            {
                var sut = CreateSchool();
                var headmasterId = sut.Members[0].Id;

                sut.DivestHeadmaster();

                sut.Members.Should().HaveCount(1);
                sut.Members[0].Role.Should().Be(Role.Teacher);
                sut.DomainEvents.Should().HaveCount(1);
                sut.DomainEvents[0].Should().BeOfType<HeadmasterDivestedDomainEvent>()
                    .Which.HeadmasterId.Should().Be(headmasterId);
            }

            [Fact]
            public void Divest_headmaster_throws_when_none_present()
            {
                var sut = CreateSchool();
                sut.DivestHeadmaster();

                var exception = Record.Exception(() => sut.DivestHeadmaster());

                Assert.IsType<InvalidOperationException>(exception);
            }
        }

        public class HeadmasterPromotionTests
        {
            [Fact]
            public void Headmaster_already_exists()
            {
                var sut = CreateSchool();
                var headmasterId = sut.Members[0].Id;
                var member = sut.EnrollMembersAs(Role.Teacher, 1)[0];

                var result = sut.PromoteHeadmaster(member.Id);

                result.IsFailure.Should().BeTrue();
                result.Error.Errors.Should().HaveCount(1);
                result.Error.Errors.Should()
                    .BeEquivalentTo($"School already have a headmaster: 'jan_nowak@gmail.com' (Id: '{headmasterId}')");
            }

            [Fact]
            public void Member_must_be_teacher_to_be_promoted()
            {
                var sut = CreateSchool();
                var member = sut.EnrollMembersAs(Role.Student, 1)[0];
                sut.DivestHeadmaster();

                var exception = Record.Exception(() => sut.PromoteHeadmaster(member.Id));

                Assert.IsType<InvalidOperationException>(exception);
            }

            [Fact]
            public void Member_cannot_be_archived_to_be_promoted()
            {
                var sut = CreateSchool();
                var member = sut.EnrollMembersAs(Role.Teacher, 1)[0];
                sut.DivestHeadmaster();
                sut.MarkMemberAsActive(member.Id);
                sut.ArchiveMember(member.Id);

                var exception = Record.Exception(() => sut.PromoteHeadmaster(member.Id));

                Assert.IsType<InvalidOperationException>(exception);

            }

            [Fact]
            public void Headmaster_is_promoted()
            {
                var sut = CreateSchool();
                var member = sut.EnrollMembersAs(Role.Teacher, 1)[0];
                sut.DivestHeadmaster();
                sut.ClearEvents();

                var result = sut.PromoteHeadmaster(member.Id);

                result.IsSuccess.Should().BeTrue();
                sut.Members[1].Role.Should().Be(Role.Headmaster);
                sut.DomainEvents.Should().HaveCount(1);
                sut.DomainEvents[0].Should().BeOfType<HeadmasterPromotedDomainEvent>();
                sut.DomainEvents[0].As<HeadmasterPromotedDomainEvent>().TeacherId.Should().Be(member.Id);
            }
        }

        public class PassOnHeadmasterTests
        {
            [Fact]
            public void Member_must_be_teacher_to_be_promoted()
            {
                var sut = CreateSchool();
                var member = sut.EnrollMembersAs(Role.Student, 1)[0];
                sut.DivestHeadmaster();

                var exception = Record.Exception(() => sut.PassOnHeadmaster(member.Id));

                Assert.IsType<InvalidOperationException>(exception);
            }

            [Fact]
            public void Member_cannot_be_archived_to_be_promoted()
            {
                var sut = CreateSchool();
                var member = sut.EnrollMembersAs(Role.Teacher, 1)[0];
                sut.DivestHeadmaster();
                sut.MarkMemberAsActive(member.Id);
                sut.ArchiveMember(member.Id);

                var exception = Record.Exception(() => sut.PassOnHeadmaster(member.Id));

                Assert.IsType<InvalidOperationException>(exception);
            }

            [Fact]
            public void Pass_on_headmaster_throws_when_none_present()
            {
                var sut = CreateSchool();
                var member = sut.EnrollMembersAs(Role.Teacher, 1)[0];
                sut.DivestHeadmaster();

                var exception = Record.Exception(() => sut.PassOnHeadmaster(member.Id));

                Assert.IsType<InvalidOperationException>(exception);
            }

            [Fact]
            public void Headmaster_is_passed()
            {
                var sut = CreateSchool();
                var member = sut.EnrollMembersAs(Role.Teacher, 1)[0];
                sut.ClearEvents();

                sut.PassOnHeadmaster(member.Id);

                sut.Members[0].Role.Should().Be(Role.Teacher);
                sut.Members[1].Role.Should().Be(Role.Headmaster);
                sut.DomainEvents.Should().HaveCount(2);
                sut.DomainEvents.OfType<HeadmasterDivestedDomainEvent>().Should().HaveCount(1).And.Subject.Single()
                    .HeadmasterId.Should().Be(sut.Members[0].Id);
                sut.DomainEvents.OfType<HeadmasterPromotedDomainEvent>().Should().HaveCount(1).And.Subject.Single()
                    .TeacherId.Should().Be(member.Id);
            }
        }

        public class GroupDeletionTests
        {
            [Fact]
            public void Group_is_deleted()
            {
                var sut = CreateSchool();
                var group = sut.AddGroups(1, 1, 'a')[0];
                var students = sut.EnrollMembersAs(Role.Student, 2);
                sut.AssignStudentToGroup(students[0].Id, group.Code);
                sut.AssignStudentToGroup(students[1].Id, group.Code);
                sut.PromoteTreasurer(group.Id, students[0].Id);
                var teacher = sut.EnrollMembersAs(Role.Teacher, 1)[0];
                sut.PromoteFormTutor(group.Id, teacher.Id);
                sut.ClearEvents();
                
                sut.DeleteGroup(group.Id);

                sut.Groups.Should().BeEmpty();
                sut.DomainEvents.Should().HaveCount(3);
                sut.Members[1].Group.Should().BeNull();
                sut.Members[2].Group.Should().BeNull();
                sut.DomainEvents.OfType<StudentDisenrolledDomainEvent>().Should().HaveCount(2);
                sut.DomainEvents.OfType<FormTutorDivestedDomainEvent>().Should().HaveCount(1);
                sut.DomainEvents.OfType<FormTutorDivestedDomainEvent>().First().FormTutorId.Should().Be(teacher.Id);
                sut.DomainEvents.OfType<StudentDisenrolledDomainEvent>()
                    .FirstOrDefault(e => e.StudentId == students[0].Id && e.RemovedRole == SK.GroupRoles.Treasurer)
                    .Should().NotBeNull();
                sut.DomainEvents.OfType<StudentDisenrolledDomainEvent>()
                    .FirstOrDefault(e => e.StudentId == students[1].Id && e.RemovedRole is null)
                    .Should().NotBeNull();
            }

            [Fact]
            public void Archived_group_is_deleted()
            {
                var sut = CreateSchool();
                var group = sut.AddGroups(3, 3, 'a')[0];
                var students = sut.EnrollMembersAs(Role.Student, 2);
                sut.AssignStudentToGroup(students[0].Id, group.Code);
                sut.AssignStudentToGroup(students[1].Id, group.Code);
                sut.PromoteTreasurer(group.Id, students[0].Id);
                var teacher = sut.EnrollMembersAs(Role.Teacher, 1)[0];
                sut.PromoteFormTutor(group.Id, teacher.Id);
                sut.MarkMemberAsActive(students[0].Id);
                sut.MarkMemberAsActive(students[1].Id);
                sut.Graduate();
                sut.ClearEvents();

                sut.DeleteGroup(group.Id);

                sut.Groups.Should().BeEmpty();
                sut.DomainEvents.Should().BeEmpty();
                sut.Members[1].Group.Should().BeNull();
                sut.Members[2].Group.Should().BeNull();
            }
        }

        public class GraduationTests
        {
            [Fact]
            public void Graduation_is_a_success()
            {
                var sut = CreateSchool();
                var groups = sut.AddGroups(2, 3, 'a');
                var students = sut.EnrollMembersAs(Role.Student, 4);
                var teachers = sut.EnrollMembersAs(Role.Teacher, 2);
                sut.AssignStudentToGroup(students[0].Id, groups[0].Code);
                sut.AssignStudentToGroup(students[1].Id, groups[0].Code);
                sut.AssignStudentToGroup(students[2].Id, groups[1].Code);
                sut.AssignStudentToGroup(students[3].Id, groups[1].Code);
                sut.PromoteTreasurer(groups[0].Id, students[0].Id);
                sut.PromoteTreasurer(groups[1].Id, students[2].Id);
                sut.PromoteFormTutor(groups[0].Id, teachers[0].Id);
                sut.PromoteFormTutor(groups[1].Id, teachers[1].Id);
                sut.MarkMemberAsActive(students[2].Id);
                sut.MarkMemberAsActive(students[3].Id);
                sut.ClearEvents();

                var result = sut.Graduate();

                result.IsSuccess.Should().BeTrue();
                sut.Groups[0].Number.Value.Should().Be(3);
                sut.Groups[0].IsArchived.Should().BeFalse();
                sut.Groups[0].FormTutor.IsArchived.Should().BeFalse();
                sut.Groups[0].Treasurer.IsArchived.Should().BeFalse();
                sut.Groups[0].Students.Should().Match(x => x.All(c => !c.IsArchived));
                sut.Groups[1].Number.Value.Should().Be(3);
                sut.Groups[1].IsArchived.Should().BeTrue();
                sut.Groups[1].FormTutor.Should().NotBeNull();
                sut.Groups[1].FormTutor.IsArchived.Should().BeFalse();
                sut.Groups[1].Treasurer.Should().NotBeNull();
                sut.Groups[1].Treasurer.IsArchived.Should().BeTrue();
                sut.Groups[1].Students.Should().Match(x => x.All(c => c.IsArchived));
                sut.DomainEvents.Should().HaveCount(3);
                sut.DomainEvents.OfType<MemberArchivedDomainEvent>().Should().HaveCount(2).And.OnlyHaveUniqueItems()
                    .And.Subject.Should().Match(x => x.All(e =>
                        (e.GroupRole == SK.GroupRoles.Treasurer && e.MemberId == students[2].Id) ||
                        (e.GroupRole == null && e.MemberId == students[3].Id)));
                sut.DomainEvents.OfType<FormTutorDivestedDomainEvent>().Should().HaveCount(1)
                    .And.Subject.Single().FormTutorId.Should().Be(teachers[1].Id);
            }

            [Fact]
            public void Graduation_fails_when_top_year_group_contain_not_active_student()
            {
                var sut = CreateSchool();
                var groups = sut.AddGroups(2, 3, 'a');
                var students = sut.EnrollMembersAs(Role.Student, 4);
                var teachers = sut.EnrollMembersAs(Role.Teacher, 2);
                sut.AssignStudentToGroup(students[0].Id, groups[0].Code);
                sut.AssignStudentToGroup(students[1].Id, groups[0].Code);
                sut.AssignStudentToGroup(students[2].Id, groups[1].Code);
                sut.AssignStudentToGroup(students[3].Id, groups[1].Code);
                sut.PromoteTreasurer(groups[0].Id, students[0].Id);
                sut.PromoteTreasurer(groups[1].Id, students[2].Id);
                sut.PromoteFormTutor(groups[0].Id, teachers[0].Id);
                sut.PromoteFormTutor(groups[1].Id, teachers[1].Id);
                sut.ClearEvents();

                var result = sut.Graduate();

                result.IsFailure.Should().BeTrue();
                result.Error.Errors.Should().BeEquivalentTo(
                    $"Cannot archive not active member 'test@o2.pl' (Id: '{students[2].Id}')!",
                    $"Cannot archive not active member 'test@o2.pl' (Id: '{students[3].Id}')!");
            }
        }

        public class MemberActivationTests
        {
            [Fact]
            public void Member_is_activated()
            {
                var sut = CreateSchool();
                var member = sut.EnrollMembersAs(Role.Student, 1)[0];
                sut.ClearEvents();

                var result = sut.MarkMemberAsActive(member.Id);

                result.IsSuccess.Should().BeTrue();
                sut.Members[1].IsActive.Should().BeTrue();
            }

            [Fact]
            public void Member_already_activated_error_is_returned()
            {
                var sut = CreateSchool();
                var member = sut.EnrollMembersAs(Role.Student, 1)[0];
                sut.MarkMemberAsActive(member.Id);
                sut.ClearEvents();

                var result = sut.MarkMemberAsActive(member.Id);

                result.IsFailure.Should().BeTrue();
                result.Error.Should().Be($"Member '{member.Id}' is already active!");
            }
        }

        private static School CreateSchool()
        {
            var name = Name.Create("II LO Rzeszów im. J. Korczaka").Value;
            var yearsOfEducation = YearsOfEducation.Create(3).Value;
            var firstName = FirstName.Create("Jan").Value;
            var lastName = LastName.Create("Nowak").Value;
            var email = Email.Create("jan_nowak@gmail.com").Value;
            var gender = Gender.Create("male").Value;

            var school = new School(name, yearsOfEducation, firstName, lastName, email, gender);
            school.ClearEvents();
            return school;
        }
    }

    internal static class SchoolTestsHelpers
    {
        public static IReadOnlyList<Member> EnrollMembersAs(this School school, Role role, int amount)
        {
            var members = new List<Member>();
            for (var i = 1; i <= amount; i++)
            {
                members.Add(school.EnrollCandidate(FirstName.Create("Jan").Value, LastName.Create("Ramsey").Value,
                    Email.Create("test@o2.pl").Value, role, Gender.Male).Value);
            }

            return members;
        }

        public static IReadOnlyList<Group> AddGroups(this School school, int startNumber, int endNumber, char sign)
        {
            var groups = new List<Group>();
            for (var i = startNumber; i <= endNumber; i++)
            {
                for (char j = 'a'; j <= sign; j++)
                    groups.Add(school.CreateGroup(Number.Create(i).Value, Sign.Create(j.ToString()).Value).Value);
            }

            return groups;
        }

        public static void EnrollHeadmaster(this School school)
        {
            school.EnrollCandidate(FirstName.Create("Jan").Value, LastName.Create("Ramsey").Value,
                Email.Create("test@o2.pl").Value, Role.Headmaster, Gender.Male);
        }
    }
}
