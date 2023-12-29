$(window).on("load", function () {

    $('#ChsContainer').on('click', '.close-chat' ,function (e) {

        var EventBtn = $(e.target);
        var uuid = EventBtn.attr("chs-uuid");
        var recipientId = EventBtn.attr("recipientId");
        const index = activeFriendChats.indexOf(recipientId);

        if (index > -1) {
            //Remove from activeFriendChats array. 
            activeFriendChats.splice(index, 1);
        }

        var chsElement = $("#chs-" + uuid);
        chsElement.remove();
    });


});

