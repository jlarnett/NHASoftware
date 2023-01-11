$(document).ready(function () {
    $("#AddFriendBtn").click(function(e) {
        var SendButton = $(e.target);
        AddFriend(SendButton);
    });
});

function AddFriend(btn) {

    var friendRequestDto = {};
    friendRequestDto.SenderUserId = btn.attr("senderId");
    friendRequestDto.RecipientUserId = btn.attr("recipientId");

    $.ajax({
        url: '/api/friend/friendrequest',
        method: 'POST',
        contentType: "application/json; charset=utf-8",
        datatype: 'json',
        data: JSON.stringify(friendRequestDto),
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function(data) {
            if (data.success) {
                console.log("Successsfully sent friendrequest");
            }
        },
        error: function (data) {
            console.log("Failed send friend request");
        }
            
    });
}