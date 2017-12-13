using System;
using System.Collections;
using System.Collections.Generic;
using Nethereum.ABI.FunctionEncoding.Attributes;
using UnityEngine;
using Nethereum.Contracts;
using Nethereum.JsonRpc.UnityClient;

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
	    StartCoroutine(GetData());
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

    // Update is called once per frame
    void Update () {
		
	}
}
