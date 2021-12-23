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
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nethermind.Core;
using Nethermind.Core.Crypto;
using Nethermind.JsonRpc;
using Nethermind.Logging;
using Nethermind.Merge.Plugin.Data.V1;

namespace Nethermind.Merge.Plugin.Handlers.V1;

public class GetPayloadBodiesV1Handler : IAsyncHandler<Keccak[], ExecutionPayloadBodyV1Result[]>
{
    private readonly IPayloadService _payloadService;
    private readonly ILogger _logger;
    
    public GetPayloadBodiesV1Handler(IPayloadService payloadService, ILogManager logManager)
    {
        _payloadService = payloadService;
        _logger = logManager.GetClassLogger();
    }

    public async Task<ResultWrapper<ExecutionPayloadBodyV1Result[]>> HandleAsync(Keccak[] blockHashes)
    {
        List<ExecutionPayloadBodyV1Result> payloadBodies = new ();
        foreach (Keccak hash in blockHashes)
        {
            Transaction[]? transactions = _payloadService.GetPayloadBody(hash);
            if (transactions is not null)
            {
                payloadBodies.Add(new ExecutionPayloadBodyV1Result(transactions));
            }
        }
        return ResultWrapper<ExecutionPayloadBodyV1Result[]>.Success(payloadBodies.ToArray());
    }
}
