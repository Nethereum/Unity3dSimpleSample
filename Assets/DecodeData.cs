using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using UnityEngine;
using Nethereum.Contracts;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.Contracts.CQS;

public class DecodeData : MonoBehaviour {

    [FunctionOutput]
    public class GetDataDTO
    {
        [Parameter("uint64", "birthTime", 1)]
        public ulong BirthTime { get; set; }

        [Parameter("string", "userName", 2)]
        public string UserName { get; set; }

        [Parameter("uint16", "starterId", 3)]
        public int StarterId { get; set; }

        [Parameter("uint16", "currLocation", 4)]
        public int CurrLocation { get; set; }

        [Parameter("bool", "isBusy", 5)]
        public bool IsBusy { get; set; }

        [Parameter("address", "owner", 6)]
        public string Owner { get; set; }

    }

    void Start ()
	{
      // StartCoroutine(GetData());
	   StartCoroutine(GetArrayUInt256());
	}

    public IEnumerator GetData()
    {
        var contractAddress = "0x786a30e1ab0c58303c85419b9077657ad4fdb0ea";
        var url = "http://localhost:8545";
        var getDataCallUnityRequest = new EthCallUnityRequest(url);
        var contract = new Contract(null, @"[{ 'constant':false,'inputs':[],'name':'getData','outputs':[{'name':'birthTime','type':'uint64'},{'name':'userName','type':'string'},{'name':'starterId','type':'uint16'},{'name':'currLocation','type':'uint16'},{'name':'isBusy','type':'bool'},{'name':'owner','type':'address'}],'payable':false,'stateMutability':'nonpayable','type':'function'}]", contractAddress);
        var function = contract.GetFunction("getData");
        var callInput = function.CreateCallInput();

        yield return getDataCallUnityRequest.SendRequest(callInput, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());
        var result = getDataCallUnityRequest.Result;
        var thing = new GetDataDTO();
        var output = function.DecodeDTOTypeOutput<GetDataDTO>(thing, result);
        Debug.Log("birth block " + output.BirthTime);
        Debug.Log("curr location " + output.CurrLocation);
        Debug.Log("busy" + output.IsBusy);
        Debug.Log("starterid " + output.StarterId);
        Debug.Log("userName " + output.UserName);
        Debug.Log("ownerAddress " + output.Owner);
    }

	[Function("GiveMeTheArray", "uint256[]")]
	public class GiveMeTheArrayFunction:ContractMessage
	{

	}

	[FunctionOutput]
	public class GiveMeTheArrayOutputDTO
	{
		[Parameter("uint256[]", "result", 1)]
		public List<BigInteger> Result {get; set;}
	}

	public IEnumerator GetArrayUInt256()
	{
		var contractAddress = "0xd0828aeb00e4db6813e2f330318ef94d2bba2f60";
		var url = "http://localhost:8545";
		var getDataCallUnityRequest = new EthCallUnityRequest(url);
		var functionMessage = new GiveMeTheArrayFunction ();
		var callInput = functionMessage.CreateCallInput(contractAddress);

		yield return getDataCallUnityRequest.SendRequest(callInput, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());
		var result = getDataCallUnityRequest.Result;

		var output = functionMessage.DecodeDTOTypeOutput<GiveMeTheArrayFunction,GiveMeTheArrayOutputDTO>(result);
		Debug.Log (output.Result [0]);
		Debug.Log (output.Result [1]);
	}
    // Update is called once per frame
    void Update () {
		
	}
}
