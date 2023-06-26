$(document).ready(function () {

    //Accesses the friend request variables needed for friend request button. 
    var isFriends = document.getElementById("FriendRequestButton").getAttribute("isFriends");
    var userSessionActive = CheckUserSessionIsActive();
    var friendRequestSent = document.getElementById("FriendRequestButton").getAttribute("friendRequestSent");

    //Initialize the friend request button.
    InitializeButton(isFriends, friendRequestSent, userSessionActive);

    //Friend Request Button click events. 
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
    //Initializes the friend request button. Called upon loading to determine starting.
    if (isFriends === "True") {
        ChangeFriendRequestButtonToFriendlyState();
    }
    else if (isFriends !== "True" && friendRequestSent === "True") {
        ChangeFriendRequestButtonToRequestedStatus();
    }
    else if (friendRequestSent !== "True" && userSessionActive === "True") {
        ChangeFriendRequestButtonStartingState();
    }
    else {
        var friendRequestButton = $('#FriendRequestButton');
        friendRequestButton.hide('fast');
    }
}

function AddFriend() {
    //Sends the Friend Request to Friend API
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
                console.log("Successsfully sent friend request to API");
            }
        },
        error: function (data) {
            console.log("Failed sending friend request to API");
        }
            
    });
}

function DeleteFriendship() {
    //Calls the friend API & removes the associated pair of friends from DB. 
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
                console.log("Successsfully deleted friendship from DB :(");
            }
        },
        error: function (data) {
            console.log("Failed sending DELETE friendship request to API.");
        }
            
    });
}

function CancelFriendRequest() {
    //Calls the friend API & cancels pending friend request. 
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
                console.log("Successsfully canceled friend request :( from DB");
            }
        },
        error: function (data) {
            console.log("Failed sending cancel friend request to friend API");
        }
            
    });
}

function ChangeFriendRequestButtonToFriendlyState() {
    //Sets the status of the Friend Request Button to 'Friends'. Handles all state specific changes
    ChangeFriendRequestIconToCheckmark();
    ShowFriendRequestButtonIcon();
    ChangeFriendButtonText("Friends");
    ClearFriendRequestLinkContainer();
    AppendFriendRequestBlockLinkToLinkContainer();
    AppendFriendRequestRemoveLinkToLinkContainer();
}

function ChangeFriendRequestButtonToRequestedStatus() {
    //Sets the status of the Friend Request Button. Handles all state specific changes
    ChangeFriendRequestIconToClock();
    ShowFriendRequestButtonIcon();
    ChangeFriendButtonText("Friend Request Sent");
    ClearFriendRequestLinkContainer();
    AppendFriendRequestCancelLinkToLinkContainer();
}

function ChangeFriendRequestButtonStartingState() {
    //Changes Button State to starting state. happens if the users are not friends, and the current user is logged into account. 
    HideFriendRequestButtonIcon();
    ChangeFriendButtonText("Send Friend Request");
    ClearFriendRequestLinkContainer();
    AppendFriendRequestAddLinkToLinkContainer();
    AppendFriendRequestBlockLinkToLinkContainer();
}


function ChangeFriendButtonText(text) {
   
    var friendButtonTextElement = $('#FriendDropdownText');
    friendButtonTextElement.text(text);
}

function ShowFriendRequestButtonIcon() {
    //Shows the friend request button main icon.
    var iconElement = $('#FriendRequestIcon');
    iconElement.slideDown();
    iconElement.show();
}

function HideFriendRequestButtonIcon() {
    //Hides the friend request button main icon.
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

function GetFriendRequestDtoFromButton() {
    //Checks the FriendRequestButton's recipientId & SenderId attribute, then returns a friendRequestDto object ready for sending to FriendAPI
    var friendRequestButton = document.getElementById("FriendRequestButton");
    var recipientId = friendRequestButton.getAttribute("recipientId");
    var senderId = friendRequestButton.getAttribute("senderId");

    var friendRequestDto = {};
    friendRequestDto.SenderUserId = senderId;
    friendRequestDto.RecipientUserId = recipientId;

    return friendRequestDto;
}

function ClearFriendRequestLinkContainer() {
    //Clears the Friend Request Link Container of all actionable links
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
    //Returns the LinkContainer element.
    var linkContainer = document.getElementById("FriendRequestDropdownLinkContainer");
    return linkContainer;
}
