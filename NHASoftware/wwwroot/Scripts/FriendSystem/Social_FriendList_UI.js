$(window).on("load", function () {

    $('#FriendListUI').on('click', '.fl-click' ,function (e) {

        let EventBtn = $(e.target);
        let friendUserId = EventBtn.attr("friend-id");

        if (friendUserId !== undefined) {
            OpenFriendChat(friendUserId);
        }
    });

    $("#ChsContainer").on('click', '.profile-link', function (e) {
        var SendButton = $(e.target);
        var userId = SendButton.attr("userId");
        var profileUrl = "/Users/GetProfiles?userId=" + userId;
        UserNavigatedToLink(profileUrl);
    });

});

var activeFriendChats = [];

function OpenFriendChat(friendUserId) {
    console.log(friendUserId);

    let friendRequestDto = {};
    friendRequestDto.SenderUserId = RetrieveCurrentUserId();
    friendRequestDto.RecipientUserId = friendUserId;
    console.log(friendRequestDto);

    if (!Utils.containsObject(friendRequestDto.RecipientUserId, activeFriendChats) && activeFriendChats.length < 5) {
        ReturnChatPartialView(friendRequestDto).then(function (data) {
            $("#ChsContainer").append(data);
            activeFriendChats.push(friendRequestDto.RecipientUserId);
        }).catch(function (response) {
            console.log("Failed to retrieve chat partial view");
        });
    }
}

function ReturnChatPartialView(friendRequestDto) {
    //Takes PostDTO data in and sends it to Home controller /ReturnPostPartialView endpoint to convert into partial view.
    return $.ajax({
        url: '/ReturnChatPartialView',
        method: 'GET',
        data: friendRequestDto,
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function(post) {

        },
        error: function (data) {

        }
    });
}