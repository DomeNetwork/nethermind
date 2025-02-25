//  Copyright (c) 2021 Demerzel Solutions Limited
//  This file is part of the Nethermind library.
// 
//  The Nethermind library is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  The Nethermind library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with the Nethermind. If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using FluentAssertions;
using Nethermind.Core.Extensions;
using Nethermind.Crypto.Blake2;
using NUnit.Framework;

namespace Nethermind.Core.Test
{
    public class Blake2Tests
    {
        private readonly Blake2Compression _blake2Compression = new();
        const string InputExceptRounds = "48c9bdf267e6096a3ba7ca8485ae67bb2bf894fe72f36e3cf1361d5f3af54fa5d182e6ad7f520e511f6c3e2b8c68059b6bbd41fbabd9831f79217e1319cde05b61626300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000300000000000000000000000000000001";

        [Test]
        [TestCase("0000000048c9bdf267e6096a3ba7ca8485ae67bb2bf894fe72f36e3cf1361d5f3af54fa5d182e6ad7f520e511f6c3e2b8c68059b6bbd41fbabd9831f79217e1319cde05b61626300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000300000000000000000000000000000001", "08c9bcf367e6096a3ba7ca8485ae67bb2bf894fe72f36e3cf1361d5f3af54fa5d282e6ad7f520e511f6c3e2b8c68059b9442be0454267ce079217e1319cde05b")]
        [TestCase("0000000c48c9bdf267e6096a3ba7ca8485ae67bb2bf894fe72f36e3cf1361d5f3af54fa5d182e6ad7f520e511f6c3e2b8c68059b6bbd41fbabd9831f79217e1319cde05b61626300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000300000000000000000000000000000001", "ba80a53f981c4d0d6a2797b69f12f6e94c212f14685ac4b74b12bb6fdbffa2d17d87c5392aab792dc252d5de4533cc9518d38aa8dbf1925ab92386edd4009923")]
        [TestCase("0000000c48c9bdf267e6096a3ba7ca8485ae67bb2bf894fe72f36e3cf1361d5f3af54fa5d182e6ad7f520e511f6c3e2b8c68059b6bbd41fbabd9831f79217e1319cde05b61626300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000300000000000000000000000000000000", "75ab69d3190a562c51aef8d88f1c2775876944407270c42c9844252c26d2875298743e7f6d5ea2f2d3e8d226039cd31b4e426ac4f2d3d666a610c2116fde4735")]
        [TestCase("0000000148c9bdf267e6096a3ba7ca8485ae67bb2bf894fe72f36e3cf1361d5f3af54fa5d182e6ad7f520e511f6c3e2b8c68059b6bbd41fbabd9831f79217e1319cde05b61626300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000300000000000000000000000000000001", "b63a380cb2897d521994a85234ee2c181b5f844d2c624c002677e9703449d2fba551b3a8333bcdf5f2f7e08993d53923de3d64fcc68c034e717b9293fed7a421")]
        [TestCase("ffffffff48c9bdf267e6096a3ba7ca8485ae67bb2bf894fe72f36e3cf1361d5f3af54fa5d182e6ad7f520e511f6c3e2b8c68059b6bbd41fbabd9831f79217e1319cde05b61626300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000300000000000000000000000000000001", "fc59093aafa9ab43daae0e914c57635c5402d8e3d2130eb9b3cc181de7f0ecf9b22bf99a7815ce16419e200e01846e6b5df8cc7703041bbceb571de6631d2615")]
        public void compression_function_should_return_valid_output(string input, string output)
        {
            byte[] blake2Result = new byte[64];
            _blake2Compression.Compress(Bytes.FromHexString(input), blake2Result);
            string? result = blake2Result.ToHexString();
            result.Should().BeEquivalentTo(output);
        }

        [TestCaseSource(nameof(TestCaseSource))]
        public void avx2_should_compute_correct_values((int Rounds, string Output) testCase)
        {
            (int rounds, string output) = testCase;
            Test(rounds, output, Blake2CompressMethod.Avx2);
        }

        [TestCaseSource(nameof(TestCaseSource))]
        public void sse41_should_compute_correct_values((int Rounds, string Output) testCase)
        {
            (int rounds, string output) = testCase;
            Test(rounds, output, Blake2CompressMethod.Sse41);
        }
        
        [TestCaseSource(nameof(TestCaseSource))]
        public void scalar_should_compute_correct_values((int Rounds, string Output) testCase)
        {
            (int rounds, string output) = testCase;
            Test(rounds, output, Blake2CompressMethod.Scalar);
        }
        
        private void Test(int rounds, string output, Blake2CompressMethod method)
        {
            string input = string.Concat(rounds.ToString("x8"), InputExceptRounds);
            
            byte[] blake2Result = new byte[64];
            _blake2Compression.Compress(Bytes.FromHexString(input), blake2Result, method);
            string result = blake2Result.ToHexString();
            result.Should().BeEquivalentTo(output);
        }
        
        public static IEnumerable<(int, string)> TestCaseSource()
        {
            yield return (0, "08c9bcf367e6096a3ba7ca8485ae67bb2bf894fe72f36e3cf1361d5f3af54fa5d282e6ad7f520e511f6c3e2b8c68059b9442be0454267ce079217e1319cde05b");
            yield return (1, "b63a380cb2897d521994a85234ee2c181b5f844d2c624c002677e9703449d2fba551b3a8333bcdf5f2f7e08993d53923de3d64fcc68c034e717b9293fed7a421");
            yield return (2, "2c96ff1bd7926f1b8bcd7824d808fdde9cf850920b625c59f1558bc608fb66a50070f53367230679e4949e7d32baac94f33af05175b7abf3b4972425a7b068ca");
            yield return (3, "b70b167bd40e83abf720fa83d014b07db1f64ae0a7c0b4d74eace08cd2515ca7927a6d6268d80043628698e31ea7d4a4f69dac2cf3ce6746825f5cff08b401cc");
            yield return (4, "0d2c9a214539ea7898029c0c95681cab88a360f633fd94ff5fae7d1e184bfab0a598296b7b046dd346ce75add0a457e3076fbc0a72ceff7eb9d4ed790d9356e9");
            yield return (5, "021e4bc08df8b11f90392a07fc4e86b0d0159d2ff06f5c329a793847e4f0c848c6aefce2d2e11ee7a73dfaadbeebfb33e3a4ad083bfd3b4e93e7b23621a97960");
            yield return (6, "a12a6af6b6d84ace0a8fcff0ae165e91b7de3bf70d9f19405e8701f2ea69ef1ed9e0206d78e61aa7867536b6982938c361e6a84ee1be15bc13b14adcd38459a1");
            yield return (7, "125dd3e4baa7f300be309deab1181db034967cc20ebecc3c0de038b0a714afaa744cea00cd843042b75c25b1d2e3931d2203111e871f35723741418117efe781");
            yield return (8, "59d8d7cbf70b0336e6f4f7a20d2ebd05f9b27ad7bb278faff380c206b68962ae630e8a4d2af1dce8a853cd722ad174e259c7ca284137fe52b61524fb5fe327f7");
            yield return (9, "69fbbdf42d5f5f2eb657faaa82862c9a492237cbb93ffd9938ff7b757671fac0a19b9f27d130b78180d070f9b9b96ee1bb1d69e2edae0c1b7602f2f2e0977614");
            yield return (10, "5a4308e0e1daede181b47775d926a6b4b6a0adf86d05bfea696fac45f08419623976bd3c786f61500b9f94a043b9dcf397e38ee237f3c273a7d812be20874f5a");
            yield return (11, "60faa8f91624b2b718210df242b788c7ae887e953dce3c7f80862bc5e4f88d827cada4d95d2c4ac41eb66b84fcdc0e12ab0c66f4d9d546ff8a0d712f324e1845");
            yield return (12, "ba80a53f981c4d0d6a2797b69f12f6e94c212f14685ac4b74b12bb6fdbffa2d17d87c5392aab792dc252d5de4533cc9518d38aa8dbf1925ab92386edd4009923");
            yield return (10_000_000, "5b6d1ca8ee5370f08008240579096021dcf8860de693cc8f5a1476ba70c3b32ba8f93c62a0b2fbcd305caaa22bc96e0dbab199a65fcd234e31404ca4b1766252");
        }
    }
}
