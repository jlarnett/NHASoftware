$(window).on("load", function () {

    $("#ChsContainer").on('click', '.profile-link', function (e) {
        e.stopPropagation();
        var SendButton = $(e.target);
        var userId = SendButton.attr("userId");
        var profileUrl = "/Users/GetProfiles?userId=" + userId;
        UserNavigatedToLink(profileUrl);
    });

    if (CheckUserSessionIsActive() === "True") {
        LoadStoredChats();
        CheckForNewMessages();
    }

});

var activeFriendChats = [];

function OpenFriendChat(friendUserId) {
    let chatWasOpened = false;
    let maxActiveChatWindows = 3;
    let friendRequestDto = {SenderUserId: RetrieveCurrentUserId(), RecipientUserId: friendUserId};

    if (CheckActiveChatsForUserId(friendRequestDto.RecipientUserId) != true) {
        if (activeFriendChats.length < maxActiveChatWindows) {
            //How to handle opening new chat when the max limit has not been reached yet. 
            chatWasOpened = OpenChat(friendRequestDto);
        }
        else {
            //When the max number of active chats is already open. How to handle it
            let removedChat = activeFriendChats.pop();
            chatWasOpened = OpenChat(friendRequestDto);

            if (chatWasOpened) {
                $("#chs-" + removedChat.chsuuid).remove();
            }
            else {
                //Chat failed to open. Add the removed element back to activeFriendChat list. 
                activeFriendChats.push(removedChat);
            }
        }
    }
    else {
        chatWasOpened = false;
    }

    return chatWasOpened;
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

function LoadStoredChats() {
    //Loads chats that were stored in the localstorage for specific user. 
    var storedChats = JSON.parse(localStorage.getItem("open_chat_userids_" + RetrieveCurrentUserId()));

    if (storedChats === null) {
        return;
    }

    SystemNotification.createNotification("Loading previous chat windows..");

    storedChats.forEach(chat => {
        OpenFriendChat(chat.recipientId);
    });
}

function CheckForNewMessages() {
    let delay = 10000
    let maxDelay = 60000

    function poll() {

        //SystemNotification.createNotification("Polling DB for new chat messages. <a type='button'>Profile</a>");
        GetNewMessagesFromChatController().done(function (response) {

            let senderUserIds = [];

            if (response.messages.length < 1 && delay < maxDelay) {
                delay *= 1.5;
            }

            response.messages.forEach(cm => {
                if (CheckActiveChatsForUserId(cm.senderUserId)) {
                    //Called when the message polling service returns a new message for the current user & they already have
                    //active ChatUI window open for message sender. E.G. Append messages to active ChatUI window
                    let chsuuid = GetChatChsUUID(cm.senderUserId);

                    GetNewMessagesFromUser(cm.senderUserId).then(function (response) {
                        $("#chs-messages-" + chsuuid).append(response);
                        PlayNotificationAudio();
                    });

                }
                else {
                    //Called when a new message is returned & ChatUI window is not already open for message sender.
                    //E.G. New ChatUI window
                    if (!Utils.containsObject(cm.senderUserId, senderUserIds)) {
                        let friendRequestDto = {SenderUserId: RetrieveCurrentUserId(), RecipientUserId: cm.senderUserId};
                        let chatWasOpened = OpenFriendChat(cm.senderUserId);
                        senderUserIds.push(cm.senderUserId);
                        if (chatWasOpened) {
                            PlayNotificationAudio();
                        }
                    }
                }

                //let friendListChatCounter = $("#Friend-chat-notification-counter-" + cm.senderUserId ).children().first().text("0");
                //let newFriendListChatCounter = parseInt(friendListChatCounter) + 1;
                //let friendListChatCounter = $("#Friend-chat-notification-counter-" + cm.senderUserId).children().first().text(newFriendListChatCounter);
                //$("#Friend-chat-notification-counter-" + cm.senderUserId).show();
            });
        }).always(function () {
            // Schedule next poll
            setTimeout(poll, delay);
        });
    }

    // Start polling
    poll();
}

function GetNewMessagesFromChatController() {
        //This returns all new messages where the current user is the recipient
        return $.ajax({
        url: '/Chat/GetNewMessagesForUser',
        method: 'GET',
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function(response) {
        },
        error: function (response) {

        }
    });
}

function GetNewMessagesFromUser(friendUserId) {
    //This returns all new messages where currerntUser is recipient & friendUserId is sender
    return $.ajax({
        url: 'Chat/NewChatsBetweenUsers/' + friendUserId,
        method: 'GET',
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function(response) {
        },
        error: function (response) {

        }
    });
}

function CheckActiveChatsForUserId(recipientId) {
    //Checks whether recipientId is within activeFriendChats open chat list. Returns true if recipientId is found within list. 
    let result = false;

    activeFriendChats.find((o, i) => {
        if(o.recipientId === recipientId) 
            result =  true;
    });

    return result;
}

function GetChatChsUUID(recipientId) {
    //Checks the activeChatUIs activeFriendChat list for recipientId & returns the chsuuid of the open chat. 
    let result = undefined;

    activeFriendChats.find((o, i) => {
        if(o.recipientId === recipientId)
            result = o.chsuuid;
    });

    return result;
}

function GetOpenChatChsUUID(recipientId) {
    //Tries to locate the open active chat for supplied recipientId. This only works if the chatUI partial view has been added
    //to the UI. 
    let chsuuid = undefined;

    let containers = document.querySelectorAll('[chs-container]');
    containers.forEach((container) => {
        let containerRecipientId = container.getAttribute("chs-recipient");

        if (containerRecipientId === recipientId) {
            chsuuid = container.getAttribute("chs-container");
        }
    });

    return chsuuid;
}

function OpenChat(friendRequestDto) {
    //Tries to open chatUI window. Takes friendRequestDTO where sender is the current user & recipient is the friend
    //They are trying to send message to. 
    let chatWasOpened = true;

    ReturnChatPartialView(friendRequestDto).then(function (data) {
        $("#ChsContainer").append(data);
        let uuid = GetOpenChatChsUUID(friendRequestDto.RecipientUserId);
        let friendChat = {chsuuid: uuid, recipientId: friendRequestDto.RecipientUserId};
        activeFriendChats.push(friendChat);
        localStorage.setItem("open_chat_userids_" + RetrieveCurrentUserId(), JSON.stringify(activeFriendChats));
        chatWasOpened = true;
    }).catch(function (response) {
        console.log("Failed to retrieve chat partial view from controller");
        chatWasOpened = false;
    });

    return chatWasOpened;
}

function PlayNotificationAudio() {
    let audio = new Audio("/Audio/ChatReceived.wav");
    audio.play();
}
