$(document).ready(function () {

    //Called whenever user clicks the friend requests actionbar icon. Displays the friend request popup modal.
    $('.friend-request-popup-btn').on('click', function() {
        var popup = document.getElementById("friend-request-popup-modal");
        popup.classList.toggle("show");
        console.log("Tried to show friend request modal.");
    });

    //Called whenever user clicks accept friend request btn.
    $('#friendRequestModal').on('click', '.accept-friend-request-btn', function (e) {
        var SendButton = $(e.target);
        var requestId = SendButton.attr("request-id");

        $.get("/api/friend/acceptfriendrequest/" + requestId, function (data) {
            if (data.success) {
                console.log("Friend Request Accepted successfully");
                RemoveFriendRequestFromModal(requestId);
                DecrementFriendRequestCounter();
            }
            else {
                console.log("Friend Request Failed WAS NOT ACCEPTED !!!");
            }
        });
    });

    //Called whenever user clicks decline friend request btn.
    $('#friendRequestModal').on('click', '.decline-friend-request-btn', function (e) {
        var SendButton = $(e.target);
        var requestId = SendButton.attr("request-id");

        $.get("/api/friend/declinefriendrequest/" + requestId, function (data) {
            if (data.success) {
                console.log("Friend Request Declined successfully");
                RemoveFriendRequestFromModal(requestId);
                DecrementFriendRequestCounter();
            }
            else {
                console.log("Friend Request Decline Failed !!!");
            }
        });
    });

    $(".Top-Action-Bar").on('click', '.profile-link', function (e) {
        var SendButton = $(e.target);
        var userId = SendButton.attr("userId");
        var profileUrl = "Users/GetProfiles?userId=" + userId;
        UserNavigatedToLink(profileUrl);
    });

    GetPendingFriendRequests();
});

function RemoveFriendRequestFromModal(requestId) {

    var friendRequestDivSection = $("div[friend-request-id$=" + requestId + "]");
    friendRequestDivSection.remove();

    var friendRequestModalBody = $('.modal-body');
    console.log(friendRequestModalBody.children().length);

    if (friendRequestModalBody.children().length > 0) {

    }
    else {
        friendRequestModalBody.append('<div>No more friend requests! Go add more people! :)</div>');
    }

}


//Gets all pending friend request for the logged in user.
const GetPendingFriendRequests = async () => {
    let = RetrieveCurrentUserId();
    const response = await fetch('/api/friend/pendingfriendrequest/' + userId);
    const myJson = await response.json(); //extract JSON from the http response
}

function DecrementFriendRequestCounter() {
    let requestCounter = $("#FriendRequestCounter");
    requestCounter.text(requestCounter.text() - 1);
}
