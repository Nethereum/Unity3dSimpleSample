using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.Extensions;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using UnityEngine;

public class EtherTransfer : MonoBehaviour {

    
    // Use this for initialization
    void Start () {

        StartCoroutine(TransferEther());
    }


    //Sample of new features / requests
    public IEnumerator TransferEther()
    {
        var url = "http://localhost:8545";
        var privateKey = "0xb5b1870957d373ef0eeffecc6e4812c0fd08f554b37b233526acc331bf1544f7";
        var account = "0x12890d2cce102216644c59daE5baed380d84830c";
        //initialising the transaction request sender
        var ethTransfer = new EthTransferUnityRequest(url, privateKey, account);
        
        var receivingAddress = "0xde0B295669a9FD93d5F28D9Ec85E40f4cb697BAe";
        yield return ethTransfer.TransferEther("0xde0B295669a9FD93d5F28D9Ec85E40f4cb697BAe", 1.1m, 2);

        if (ethTransfer.Exception != null)
        {
            Debug.Log(ethTransfer.Exception.Message);
            yield break;
        }

        var transactionHash = ethTransfer.Result;

        Debug.Log("Transfer transaction hash:" + transactionHash);

        //create a poll to get the receipt when mined
        var transactionReceiptPolling = new TransactionReceiptPollingRequest(url);
        //checking every 2 seconds for the receipt
        yield return transactionReceiptPolling.PollForReceipt(transactionHash, 2);
        
        Debug.Log("Transaction mined");

        var balanceRequest = new EthGetBalanceUnityRequest(url);
        yield return balanceRequest.SendRequest(receivingAddress, BlockParameter.CreateLatest());
        
        
        Debug.Log("Balance of account:" + UnitConversion.Convert.FromWei(balanceRequest.Result.Value));
    }



    // Update is called once per frame
    void Update () {
		
	}
}
