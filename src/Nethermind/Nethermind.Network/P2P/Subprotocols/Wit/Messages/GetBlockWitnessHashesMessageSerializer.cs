﻿//  Copyright (c) 2021 Demerzel Solutions Limited
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

using DotNetty.Buffers;
using Nethermind.Serialization.Rlp;

namespace Nethermind.Network.P2P.Subprotocols.Wit.Messages
{
    public class GetBlockWitnessHashesMessageSerializer : IZeroInnerMessageSerializer<GetBlockWitnessHashesMessage>
    {
        public void Serialize(IByteBuffer byteBuffer, GetBlockWitnessHashesMessage message)
        {
            NettyRlpStream nettyRlpStream = new(byteBuffer);
            int totalLength = GetLength(message, out int contentLength);
            byteBuffer.EnsureWritable(totalLength, true);
            nettyRlpStream.StartSequence(contentLength);
            nettyRlpStream.Encode(message.RequestId);
            nettyRlpStream.Encode(message.BlockHash);
        }

        public int GetLength(GetBlockWitnessHashesMessage message, out int contentLength)
        {
            contentLength = Rlp.LengthOf(message.RequestId)
                            + (message.BlockHash is null ? 1 : Rlp.LengthOfKeccakRlp);
            return Rlp.LengthOfSequence(contentLength) + Rlp.LengthOf(message.RequestId) + Rlp.LengthOf(message.BlockHash);
        }

        public GetBlockWitnessHashesMessage Deserialize(IByteBuffer byteBuffer)
        {
            NettyRlpStream rlpStream = new(byteBuffer);
            rlpStream.ReadSequenceLength();
            long requestId = rlpStream.DecodeLong();
            var hash = rlpStream.DecodeKeccak();
            return new GetBlockWitnessHashesMessage(requestId, hash);
        }
    }
}
