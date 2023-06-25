$(document).ready(function () {

    var isFriends = document.getElementById("FriendRequestButton").getAttribute("isFriends");
    var userSessionActive = CheckUserSessionIsActive();
    var friendRequestSent = document.getElementById("FriendRequestButton").getAttribute("friendRequestSent");

    InitializeButton(isFriends, friendRequestSent, userSessionActive);

    $("#FriendRequestButton").on("click", ".add-friend", function (e) { 
        console.log("Trying to add friend........");
        AddFriend();
    });

    $("#FriendRequestButton").on("click", ".remove-friend", function (e) {
        console.log("Trying to remove friend........");
        DeleteFriendship();
    });

    $("#FriendRequestButton").on("click", ".cancel-friend", function (e) {
        console.log("Trying to cancel friend request........");
        CancelFriendRequest();
    });
});

function InitializeButton(isFriends, friendRequestSent, userSessionActive) {
    
    if (isFriends === "True") {
        //Users are friends
        console.log("Trying to initialize friend button - Friendly State");
        ChangeFriendRequestButtonToFriendlyState();
    }
    else if (isFriends !== "True" && friendRequestSent === "True") {
        //Friend request is sent, but not accepted yet
        console.log("Trying to initialize friend button - Friend Request Sent State");
        ChangeFriendRequestButtonToRequestedStatus();
    }
    else if (friendRequestSent !== "True" && userSessionActive === "True") {
        //User is logged in, but friend request was NOT sent to user yet. 
        console.log("Trying to initialize friend button - User Logged In & request not sent. ");
        ChangeFriendRequestButtonStartingState();
    }
    else {
        //Hide Friend Request Button
    }
}

function AddFriend() {
    var friendRequestDto = GetFriendRequestDtoFromButton();

    $.ajax({
        url: '/api/friend/friendrequest',
        method: 'POST',
        contentType: "application/json; charset=utf-8",
        datatype: 'json',
        data: JSON.stringify(friendRequestDto),
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function(data) {
            if (data.success) {
                ChangeFriendRequestButtonToRequestedStatus();
                console.log("Successsfully sent friendrequest");
            }
        },
        error: function (data) {
            console.log("Failed send friend request");
        }
            
    });
}

function DeleteFriendship() {
    var friendRequestDto = GetFriendRequestDtoFromButton();

    $.ajax({
        url: '/api/friend/DeleteFriendship',
        method: 'DELETE',
        contentType: "application/json; charset=utf-8",
        datatype: 'json',
        data: JSON.stringify(friendRequestDto),
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function(data) {
            if (data.success) {
                ChangeFriendRequestButtonStartingState();
                console.log("Successsfully deleted friendship :(");
            }
        },
        error: function (data) {
            console.log("ERROR ERROR unable to remove friendship from DB");
        }
            
    });
}

function CancelFriendRequest() {
    var friendRequestDto = GetFriendRequestDtoFromButton();

    $.ajax({
        url: '/api/friend/CancelFriendRequest',
        method: 'PUT',
        contentType: "application/json; charset=utf-8",
        datatype: 'json',
        data: JSON.stringify(friendRequestDto),
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function(data) {
            if (data.success) {
                ChangeFriendRequestButtonStartingState();
                console.log("Successsfully canceled friend request :(");
            }
        },
        error: function (data) {
            console.log("ERROR ERROR unable to cancel friend request");
        }
            
    });
}

//Sets the status of the Friend Request Button to 'Friends'. Handles all state specific changes
function ChangeFriendRequestButtonToFriendlyState() {

    ChangeFriendRequestIconToCheckmark();
    ShowFriendRequestButtonIcon();

    
    ChangeFriendButtonText("Friends");

    ClearFriendRequestLinkContainer();
    AppendFriendRequestBlockLinkToLinkContainer();
    AppendFriendRequestRemoveLinkToLinkContainer();
}

//Sets the status of the Friend Request Button. Handles all state specific changes
function ChangeFriendRequestButtonToRequestedStatus() {

    ChangeFriendRequestIconToClock();
    //Hide Checkmark
    ShowFriendRequestButtonIcon();

    //Change the dropdown main button text. 
    ChangeFriendButtonText("Friend Request Sent");

    //Clear Action Links
    ClearFriendRequestLinkContainer();
    AppendFriendRequestCancelLinkToLinkContainer();

    
}

//Initial Button State if the users are not friends, and the current user is logged into account. 
function ChangeFriendRequestButtonStartingState() {
    //Hide Checkmark
    HideFriendRequestButtonIcon();
    //Change Button Text
    ChangeFriendButtonText("Send Friend Request");
    //Clear Request Link Container
    ClearFriendRequestLinkContainer();
    //Append action links
    AppendFriendRequestAddLinkToLinkContainer();
    AppendFriendRequestBlockLinkToLinkContainer();
}

function ChangeFriendButtonText(text) {
    var friendButtonTextElement = $('#FriendDropdownText');
    friendButtonTextElement.text(text);
}

function ShowFriendRequestButtonIcon() {
    var iconElement = $('#FriendRequestIcon');
    iconElement.slideDown();
    iconElement.show();
}

function HideFriendRequestButtonIcon() {
    var iconElement = $('#FriendRequestIcon');
    iconElement.slideUp();
    iconElement.hide('slow');
}

function ChangeFriendRequestIconToClock() {
    var iconElement = $('#FriendRequestIcon');
    iconElement.attr("src", "/Images/Clock_Icon.png");
}

function ChangeFriendRequestIconToCheckmark() {
    var iconElement = $('#FriendRequestIcon');
    iconElement.attr("src", "/Images/CheckMarkIcon.png");
}

//Checks the FriendRequestButton's recipientId & SenderId attribute, then returns a friendRequestDto object ready for sending to FriendAPI
function GetFriendRequestDtoFromButton() {
    var friendRequestButton = document.getElementById("FriendRequestButton");
    var recipientId = friendRequestButton.getAttribute("recipientId");
    var senderId = friendRequestButton.getAttribute("senderId");

    var friendRequestDto = {};
    friendRequestDto.SenderUserId = senderId;
    friendRequestDto.RecipientUserId = recipientId;

    return friendRequestDto;
}

//Clears the Friend Request Link Container of all actionable links
function ClearFriendRequestLinkContainer() {
    var linkContainer = $('#FriendRequestDropdownLinkContainer');
    linkContainer.empty();
}

function AppendFriendRequestRemoveLinkToLinkContainer() {
    var linkContainer = ReturnFriendRequestDropdownLinkContainer();
    linkContainer.innerHTML += '<li><a class="dropdown-item remove-friend">Remove Friend</a></li>';
}

function AppendFriendRequestAddLinkToLinkContainer() {
    var linkContainer = ReturnFriendRequestDropdownLinkContainer();
    linkContainer.innerHTML += '<li><a class="dropdown-item add-friend"><img src="/Images/plus_icon.png"/>Add Friend!</a></li>';
}

function AppendFriendRequestBlockLinkToLinkContainer() {
    var linkContainer = ReturnFriendRequestDropdownLinkContainer();
    linkContainer.innerHTML += '<li><a class="dropdown-item block-friend">Block Ex-Friend</a></li>';
}

function AppendFriendRequestCancelLinkToLinkContainer() {
    var linkContainer = ReturnFriendRequestDropdownLinkContainer();
    linkContainer.innerHTML += '<li><a class="dropdown-item cancel-friend">Cancel Friend Request</a></li>';
}

function ReturnFriendRequestDropdownLinkContainer() {
    var linkContainer = document.getElementById("FriendRequestDropdownLinkContainer");
    return linkContainer;
}
