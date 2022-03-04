function AddHandle(e)
{
    e.preventDefault();
    let twitterHandleList = document.getElementById("multiTweet");
    console.log(twitterHandleList);
    let groups = twitterHandleList.querySelectorAll(".input-group");
    if(groups.length < 10)
    {
        document.querySelector("#errOut").innerText = "";
        let newInput = `<div class="input-group input-group-lg w-50 mx-auto mb-3">
        <div class="input-group-prepend">
        <span class="input-group-text" id="basic-addon1">@</span>
        </div>
        <input name="MultiHandle[${groups.length}]" placeholder="Username" aria-label="Username" aria-describedby="basic-addon1" class="form-control" type="text" id="MultiHandle" value="">
        </div>`
        $("#multiTweet").append(newInput);
    }else
    {
        document.querySelector("#errOut").innerText = "Your scientific hubris blinds you! Keep it under eleven ingredients."
    }
}

function RemoveHandle(e)
{
    e.preventDefault();
    let twitterHandleList = document.getElementById("multiTweet");
    let groups = twitterHandleList.querySelectorAll(".input-group");
    if(groups.length > 2)
    {
        document.querySelector("#errOut").innerText = "";
        groups[groups.length-1].remove();
    }
    else
        document.querySelector("#errOut").innerText = "To combine essences together we require at least Two!"

}


document.getElementById("addHandle").addEventListener("click", AddHandle);
document.getElementById("removeHandle").addEventListener("click", RemoveHandle);
