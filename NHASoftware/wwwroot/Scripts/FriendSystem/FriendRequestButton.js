$(document).ready(function () {

    //Accesses the friend request variables needed for friend request button. 
    let sameUser = ProfileIsSameUser();
    let isFriends = document.getElementById("FriendRequestButton").getAttribute("isFriends");
    let userSessionActive = CheckUserSessionIsActive();
    let friendRequestSent = document.getElementById("FriendRequestButton").getAttribute("friendRequestSent");

    //Initialize the friend request button.
    InitializeButton(isFriends, friendRequestSent, userSessionActive, sameUser);

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

function InitializeButton(isFriends, friendRequestSent, userSessionActive, sameUser) {
    //Initializes the friend request button. Called upon loading to determine starting.

    if (isFriends === "True") {
        ChangeFriendRequestButtonToFriendlyState();
    }
    else if (isFriends !== "True" && friendRequestSent === "True" && sameUser !== true) {
        ChangeFriendRequestButtonToRequestedStatus();
    }
    else if (sameUser === true && userSessionActive === "True") {
        HideFriendRequestButton();
    }
    else if (friendRequestSent !== "True" && userSessionActive === "True" && sameUser !== true) {
        ChangeFriendRequestButtonStartingState();
    }
    else {
        HideFriendRequestButton();
    }
}

function AddFriend() {
    //Sends the Friend Request to Friend API
    let friendRequestDto = GetFriendRequestDtoFromButton();

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
    let friendRequestDto = GetFriendRequestDtoFromButton();

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
    let friendRequestDto = GetFriendRequestDtoFromButton();

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
    let friendButtonTextElement = $('#FriendDropdownText');
    friendButtonTextElement.text(text);
}

function ShowFriendRequestButtonIcon() {
    //Shows the friend request button main icon.
    let iconElement = $('#FriendRequestIcon');
    iconElement.slideDown();
    iconElement.show();
}

function HideFriendRequestButtonIcon() {
    //Hides the friend request button main icon.
    let iconElement = $('#FriendRequestIcon');
    iconElement.slideUp();
    iconElement.hide('slow');
}

function ChangeFriendRequestIconToClock() {
    let iconElement = $('#FriendRequestIcon');
    iconElement.attr("src", "/Images/Clock_Icon.png");
}

function ChangeFriendRequestIconToCheckmark() {
    let iconElement = $('#FriendRequestIcon');
    iconElement.attr("src", "/Images/CheckMarkIcon.png");
}

function GetFriendRequestDtoFromButton() {
    //Checks the FriendRequestButton's recipientId & SenderId attribute, then returns a friendRequestDto object ready for sending to FriendAPI
    let friendRequestButton = document.getElementById("FriendRequestButton");
    let recipientId = friendRequestButton.getAttribute("recipientId");
    let senderId = friendRequestButton.getAttribute("senderId");

    var friendRequestDto = {};
    friendRequestDto.SenderUserId = senderId;
    friendRequestDto.RecipientUserId = recipientId;

    return friendRequestDto;
}

function ClearFriendRequestLinkContainer() {
    //Clears the Friend Request Link Container of all actionable links
    let linkContainer = $('#FriendRequestDropdownLinkContainer');
    linkContainer.empty();
}

function AppendFriendRequestRemoveLinkToLinkContainer() {
    let linkContainer = ReturnFriendRequestDropdownLinkContainer();
    linkContainer.innerHTML += '<li><a class="dropdown-item remove-friend">Remove Friend</a></li>';
}

function AppendFriendRequestAddLinkToLinkContainer() {
    let linkContainer = ReturnFriendRequestDropdownLinkContainer();
    linkContainer.innerHTML += '<li><a class="dropdown-item add-friend"><img src="/Images/plus_icon.png"/>Add Friend!</a></li>';
}

function AppendFriendRequestBlockLinkToLinkContainer() {
    let linkContainer = ReturnFriendRequestDropdownLinkContainer();
    linkContainer.innerHTML += '<li><a class="dropdown-item block-friend">Block Ex-Friend</a></li>';
}

function AppendFriendRequestCancelLinkToLinkContainer() {
    let linkContainer = ReturnFriendRequestDropdownLinkContainer();
    linkContainer.innerHTML += '<li><a class="dropdown-item cancel-friend">Cancel Friend Request</a></li>';
}

function ReturnFriendRequestDropdownLinkContainer() {
    //Returns the LinkContainer element.
    let linkContainer = document.getElementById("FriendRequestDropdownLinkContainer");
    return linkContainer;
}

function HideFriendRequestButton() {
    let friendRequestButton = $('#FriendRequestButton');
    friendRequestButton.hide('fast');
}

function ProfileIsSameUser() {
    let dto = GetFriendRequestDtoFromButton();

    if (dto.SenderUserId === dto.RecipientUserId) {
        return true;
    }
    else {
        return false;
    }
}
