$(window).on("load", function () {

    $('#ChsContainer').on('click', '.close-chat' ,function (e) {

        var EventBtn = $(e.target);
        var uuid = EventBtn.attr("chs-uuid");
        var recipientId = EventBtn.attr("recipientId");

        const index = activeFriendChats.map(e => e.recipientId).indexOf(recipientId);

        if (index > -1) {
            //Remove from activeFriendChats array. 
            activeFriendChats.splice(index, 1);
        }

        localStorage.setItem("open_chat_userids_" + RetrieveCurrentUserId(), JSON.stringify(activeFriendChats));

        var chsElement = $("#chs-" + uuid);
        chsElement.remove();
    });

    $('#ChsContainer').on('click', '.submit-chat' ,function (e) {
        e.preventDefault();
        var EventBtn = $(e.target);
        var uuid = EventBtn.attr("chs-uuid");

        let form = document.getElementById("chs-form-" + uuid);
        let formData = new FormData(form);

        SendChat('/Chat/SendMessage', formData).then(function (response) {
            $("#chs-messages-" + uuid).append(response);
            $("#chs-form-summary-" + uuid).val('');
        }).catch(function (response) {

        });
    });

    $('#FriendListUI').on('click', '.open-chat' ,function (e) {

        let EventBtn = $(e.target);
        let friendUserId = EventBtn.attr("friend-id");

        if (friendUserId !== undefined) {
            OpenFriendChat(friendUserId);
            $("#Friend-chat-notification-counter-" + friendUserId).hide();
        }
    });


});

function SendChat(ApiUrl, postForm) {
    //Generic method for sending post objects to API for creation. Takes in the API endpoint & the postForm to send. 
    return $.ajax({
        url: ApiUrl,
        method: 'POST',
        contentType: false,
        processData: false,
        data: postForm,
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function(data) {
            if (data.success) {
            }
        },
        error: function (data) {
        }
    });


}

$(document).on('click', '.chat-header', function () {
    const container = $(this).closest('[chs-container]');
    const uuid = container.attr('chs-container');

    const body = container.find('[chs-body-uuid="' + uuid + '"]');

    body.slideToggle(200);
});