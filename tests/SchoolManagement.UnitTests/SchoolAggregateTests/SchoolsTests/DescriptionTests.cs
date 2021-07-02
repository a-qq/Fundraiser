using System.Collections.Generic;
using FluentAssertions;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using Xunit;

namespace SchoolManagement.UnitTests.SchoolAggregateTests.SchoolsTests
{
    public class DescriptionTests
    {
        [Fact]
        public void Description_with_too_long_text_is_invalid()
        {
            const string inputDescription =
                "hxboljgjsnkbceqktuzsxstepcgomlbrljgpvgqvrdaiodzmjatqrgztcnctbgolmkdzoykmsjrsdvroyupnuthrsvxpcoykniybqhcxbucbgueegfskykcvxdcleoqmhfhhelfgtdecykuctigxmsoequcxrvnlljnhodxnsplyjwyifxssgnffxlgpapbfcixlxtpbnagtxdzckciormwuqivfufkiutmbmlwaeiybxvinauflcsjuiaavybqcuzubifjaklpbvzcfrmefaifzmphzirwhhpcyrcevvzqnvayzckfsogjhgznlnhrhpszvnyjfzjydoqqtajjetohkmjrinzqgvaxsuscqtjtqzeltkfyosdjkygxqvyklkxsxeqtgydwimnmfufjozbqsqlyuvbgjapzjimygafrgfrdnugqezjximltqpyiqalpyqexwfnouxikgcwiktzynizlcsribkbertysebbdcpcxjmipfodojfznkdvdsgkjaageeoqvxuukmpdbqnshdfhzgydttsdfmsxmdiituvzxqujymoflvuwdrbplqwnzcwkmwjzauugusdeoeidmdruiguuriyzpwicorpudlpllwbyrcsenrljnkgjmwufqyyaeyadbyqvlhpgasitslvkzclwfsmavusgprzquhdhhjugwhfwfzevnrqchhjoetobpbfaakepgmnopclbvusaslxeqszcoixqaaymuaagoaprbuvjtwyimavpxddtrjrtdixyzpfdisyipgbsqltucxbzgpyqcoaootynjthlxgejnlaksaicdgzjgtyxtuttdpxdlemdvywbfpwqvjgddgrqojboxugdlvcsiiacqyzopcldvfhacqevapioearezixzhirzxgtzxkvhqmyfssjhrixgecrsihqszjvasmypptirxopdzhqeoqnjlzwklhilmeavtbbrfmnvdetogsckjoodxybgqohtypqzbnddyuvrdwwvrzhvvnqzifqndqrizwosihggkufauejogbfdqhlqqstzvabrtxmixpljtjadhvllkfotoievrjfhwqxuvltvevdhgglfjdbakqdrcnldvvocztjywcrizolulkdoghrmkjdcujtkamxuilozrhfxwnfzoybzrehmkpjxeovuxhchafubbjaggrpxyfazepvpkoybhkcnehwsgvoqdtzmtjbecmxwnqvltfaxkdfjkuzcnkwqczwlqovohkhzoyaqhadjwylvqenlhfzddhkmwplxdalsgvbdwvqwycvmsgtfcjzkrksjcevtjgsihzubtruguxjshvqnrfyfbcduczapwxkgpwjoissydzqnawaexpkuzqcuwpknzigjqskpadwedzreodvwmeoprktvdwvustpplcvhxzfafckeiabznvufpvepcxmbtejwuidfkzhkzixluuieduucuykbceupdybzcztduimqlhjnlxcspnvrmiplohvaapbsbgaxqzlbnvgwxwwulwuskjpjnwmzaiplazlrklgiryobqhnjuhzpdlzizwqyojzkbahnyfkryecnvtoyrxsjaqogriphbrxgwjibdrfalpnusfjbbzinakuupswnplcrviitozyhodlyjnsahcjxheemjlzoadhyidpukjjeapxwkikqxmboghkewyjbksdondayrrwjjhlfvmpeudodzmaxphjvqazqslptapipfvmbgkkqouxlmxlgwxmxnoyjpnwrfnkajpdbroihnaekrhgxrwpnazabpzpduqikbczrhsrejlrpfubhjhmvubdidesjlpsvhvvfbfbmzrqvazfnsoytjuatzwjwuwizipscymbfkagzvabbfyewebeyhsjybnwxfxeqxzxqrcydxmmgzkiwtgrksnwggackdpxdhacrtostqjzhrmtfbpiorqiieuslcosakksxtitupyokbmvhziobnvhlxneibimszfojsnyvyrgjdauctbvrjxscpghyhqxfjxjbivldeslsyqgosyfmmmyffwukijmgfppwjdpbaqiqiaszjhkwmqvozbrbfhmtdqdbznbthaawnhuhmlfmhvwfdztlvuflgdjzzxwutgtzbuxxglswpnrfbquvlqfwcgshdmkrtslgflmhrwrlrulytwiiukedpxnehuhvmtfyymfwsslsylnyfwhqlhagoousigtciibduflojmuarvtfvshjzoaakejsninnsueqlfkgramagrnfajinswbusffhospxcclepsadiqkvnugucnqpgsguyiudddpjpzrkzrmkzvdsrvyqyderdwglznzmmetaqihqhertvossbgsbhqezhvkpiiddwrlnanioiacfrpmpcuxzlqkawydxotqrilborlcytthvuklfqskzrpginfxhkskpeasmyugqaovurwrkwfxifnfrqxnwcvembwekgwqgcfzbhqnduxkxqjbjpncdbysjjyyxoenwqdmzucfqdtnykddwpgfecimyvqeusjrmgxgqlhkkquvtlriusybkhyaictbhlhybkdijlhwjpcovsgegzcawrkkmdsmigrbavcqraeomfmusmioqkcvroqmnfcuinwzvscbnddswuuxgeceuzdlcmghfcweskdknbyxunbpnokeddevcnhconsopylfjiokyiekxypjhjqdyrtlsqzylvfrhukzmasoqfuertkamvgqqzuavehgmifjpbevxewkmgkwnfyunkyyeyxhaedhxvywjhzyqxgjuleeakqbgkmsaahwndgfhrzshrxvwgxbtntpqsslshzyprmcrrvxsgwvdxoaawbamqpzavhlodwvxnxxnzngexxympwdjomwkufcixkderyzxjxdcnsppnndynhkuvaxuvxfbhggzombeyycfvjjvedtuimcqtlpuxmfkaygozqa";
            const string propertyName = "SchoolDescription";

            var sut = Description.Create(inputDescription, propertyName);

            sut.IsFailure.Should().BeTrue();
            sut.Error.Should().Be(MaxLengthExceededError(propertyName));
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void Creates_valid_description(string inputDescription, string outputDescription)
        {
            var sut = Description.Create(inputDescription);

            sut.IsSuccess.Should().BeTrue();
            sut.Value.Value.Should().Be(outputDescription);
        }

        public static List<object[]> Data()
        {
            return new List<object[]>
            {
                new object[] {" exampleDescription ", "exampleDescription"},
                new object[] {"     ", null},
                new object[] {string.Empty, null},
                new object[] {null, null}
            };
        }

        static string MaxLengthExceededError(string propertyName = nameof(Description))
            => $"{propertyName} should contain max {Description.MaxLength} characters!";
    }
}
