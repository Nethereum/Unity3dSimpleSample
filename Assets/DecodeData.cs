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

    public class ArrayUint256DynamicDeployment : ContractDeploymentMessage
    {
        public static string BYTECODE = "608060405234801561001057600080fd5b50610127806100206000396000f300608060405260043610603e5763ffffffff7c01000000000000000000000000000000000000000000000000000000006000350416634b04bc0481146043575b600080fd5b348015604e57600080fd5b50605560a3565b60408051602080825283518183015283519192839290830191858101910280838360005b83811015608f5781810151838201526020016079565b505050509050019250505060405180910390f35b6040805160028082526060808301845292602083019080388339019050509050600181600081518110151560d357fe5b6020908102909101015280516002908290600190811060ee57fe5b60209081029091010152905600a165627a7a72305820aa3f79a3ff9e580c10090fb0e8830490f2acbd918c76c8488c40bdaf1c9180800029";
        public ArrayUint256DynamicDeployment() : base(BYTECODE) { }
        public ArrayUint256DynamicDeployment(string byteCode) : base(byteCode) { }
    }

    [Function("GiveMeTheArray", "uint256[]")]
	public class GiveMeTheArrayFunction:FunctionMessage
	{

	}

	[FunctionOutput]
	public class GiveMeTheArrayOutputDTO:IFunctionOutputDTO
	{
		[Parameter("uint256[]", "result", 1)]
		public List<BigInteger> Result {get; set;}
	}

    //Sample of new features / requests
	public IEnumerator GetArrayUInt256()
    { 
		var url = "http://localhost:8545";
        var privateKey = "0xb5b1870957d373ef0eeffecc6e4812c0fd08f554b37b233526acc331bf1544f7";
        var account = "0x12890d2cce102216644c59daE5baed380d84830c";
        //initialising the transaction request sender
        var transactionRequest = new TransactionSignedUnityRequest(url, privateKey, account);
        //deploy the contract and True indicates we want to estimate the gas
        yield return transactionRequest.SignAndSendDeploymentContractTransaction<ArrayUint256DynamicDeployment>(true);

        if (transactionRequest.Exception != null)
        {
           Debug.Log(transactionRequest.Exception.Message);
           yield break;
        }

        var transactionHash = transactionRequest.Result;

        //create a poll to get the receipt when mined
        var transactionReceiptPolling = new TransactionReceiptPollingRequest(url);
        //checking every 2 seconds for the receipt
        yield return transactionReceiptPolling.PollForReceipt(transactionHash, 2);
        var deploymentReceipt = transactionReceiptPolling.Result;

        //Query request using our acccount and the contracts address (no parameters needed and default values)
        var queryRequest = new QueryUnityRequest<GiveMeTheArrayFunction,GiveMeTheArrayOutputDTO>(url, account);
        yield return queryRequest.Query(deploymentReceipt.ContractAddress);

        //Getting the dto response already decoded
		var dtoResult = queryRequest.Result;

		Debug.Log (dtoResult.Result [0]);
		Debug.Log (dtoResult.Result [1]);
	}
    // Update is called once per frame
    void Update () {
		
	}
}
