# Unity3dSimpleSample

This simple sample demonstrates how retrieve multiple outputs from a smart contract call and output the result to the Debug Window. If deploying to Webgl the output will be visible on the console.

## Solidity contract
The solidity contract just outputs different data as follows when call getData.
```javascript
pragma solidity ^0.4.19;

            contract TestOutput {

                function getData() returns (uint64 birthTime, string userName, uint16 starterId, uint16 currLocation, bool isBusy, address owner ) {
                    birthTime = 1;
                    userName = "juan";
                    starterId = 1;
                    currLocation = 1;
                    isBusy = false;
                    owner = 0xde0b295669a9fd93d5f28d9ec85e40f4cb697bae;
                }
            }
```
## Deployment

You will need to deploy it first as follows to your testchain:

```csharp
 var string ABI = @"[{'constant':false,'inputs':[],'name':'getData','outputs':[{'name':'birthTime','type':'uint64'},{'name':'userName','type':'string'},{'name':'starterId','type':'uint16'},{'name':'currLocation','type':'uint16'},{'name':'isBusy','type':'bool'},{'name':'owner','type':'address'}],'payable':false,'stateMutability':'nonpayable','type':'function'}]";
var string BYTE_CODE = "0x6060604052341561000f57600080fd5b6101c88061001e6000396000f3006060604052600436106100405763ffffffff7c01000000000000000000000000000000000000000000000000000000006000350416633bc5de308114610045575b600080fd5b341561005057600080fd5b61005861011a565b60405167ffffffffffffffff8716815261ffff808616604083015284166060820152821515608082015273ffffffffffffffffffffffffffffffffffffffff821660a082015260c06020820181815290820187818151815260200191508051906020019080838360005b838110156100da5780820151838201526020016100c2565b50505050905090810190601f1680156101075780820380516001836020036101000a031916815260200191505b5097505050505050505060405180910390f35b600061012461018a565b6000806000806001955060408051908101604052600481527f6a75616e0000000000000000000000000000000000000000000000000000000060208201529596600195508594506000935073de0b295669a9fd93d5f28d9ec85e40f4cb697bae92509050565b602060405190810160405260008152905600a165627a7a72305820ba7625d1c6f0f2844d32ad76e28729e80979f69cbd32d0589995f24cb969a6850029";
      
var senderAddress = "0x12890d2cce102216644c59daE5baed380d84830c";
var privateKey = "0xb5b1870957d373ef0eeffecc6e4812c0fd08f554b37b233526acc331bf1544f7";
var web3 = new Web3(new Account(privateKey));
var contractReceipt = await web3.Eth.DeployContract.SendRequestAndWaitForReceiptAsync(ABI,
BYTE_CODE, senderAddress, new HexBigInteger(900000));
```
## The Unity3d code

To retrieve multiple parameters we need a DTO object to represent all the output parameters:

```csharp
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
```
To retrieve the data we will create an EthCallUnityRequest, which it will be used to make the call.

Functions in Unity3d are used to build the call input, so first we will create a contract with the ABI and contract address to create a Function instance to create the call input. There are not parameters in this function so it is as simple as ```function.CreateCallInput();```

The next step is to make the call and retrieve the result
```
  yield return getDataCallUnityRequest.SendRequest(callInput, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());
  var result = getDataCallUnityRequest.Result;
```
Finally we will just deserialise the result into our object.
```
var output = function.DecodeDTOTypeOutput<GetDataDTO>(result);
```

#### Full sample

```csharp
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
        
        var output = function.DecodeDTOTypeOutput<GetDataDTO>(result);
        Debug.Log("birth block " + output.BirthTime);
        Debug.Log("curr location " + output.CurrLocation);
        Debug.Log("busy" + output.IsBusy);
        Debug.Log("starterid " + output.StarterId);
        Debug.Log("userName " + output.UserName);
        Debug.Log("ownerAddress " + output.Owner);
    }


```
