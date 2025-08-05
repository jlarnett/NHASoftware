$(document).ready(function () {

    $("#FriendListTable").on("click", ".remove-friend", function () {

        //Getting required elements
        let button = $(this);
        let removeFriendModal = $("#DeleteFriendModalBodyDescription");
        let removeFriendBtn = $("#RemoveFriendBtn");

        //Setting the remove friend modal text & giving it the recipient user Id value
        let friendToRemoveDisplayName = button.attr("friend-user-displayname");
        removeFriendBtn.attr("recipient-user-id", button.attr("friend-user-id"));
        removeFriendModal.text("Are you sure you want to remove " + friendToRemoveDisplayName + " from friends list?");
    });

    $("#RemoveFriendBtn").on("click", function () {
        //Send Delete Friend API Call
        let removeFriendBtn = document.getElementById("RemoveFriendBtn");
        let recipientId = removeFriendBtn.getAttribute("recipient-user-id");
        DeleteFriendship(recipientId);
    });
});

function DeleteFriendship(recipientId) {
    //Calls the friend API & removes the associated pair of friends from DB. 

    let removeFriendBtn = document.getElementById("RemoveFriendBtn");
    let senderId = removeFriendBtn.getAttribute("sender-user-id");

    var friendRequestDto = {};
    friendRequestDto.SenderUserId = senderId;
    friendRequestDto.RecipientUserId = recipientId;

    $.ajax({
        url: '/api/friend/DeleteFriendship',
        method: 'DELETE',
        contentType: "application/json; charset=utf-8",
        datatype: 'json',
        data: JSON.stringify(friendRequestDto),
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function(data) {
            if (data.success) {
                console.log("Successsfully deleted friendship from DB :(");
                //Dynamically remove the deleted friend from datatable
                RemoveFriendListTableRow(recipientId);
            }
        },
        error: function (data) {
            console.log("Failed sending DELETE friendship request to API.");
        }
    });
}

function RemoveFriendListTableRow(recipientId) {
    //Dynamically removes the friend row from data table using recipientUserId to locate the row

    //Hides the remove friend confirmation modal from screen. 
    var myModalEl = document.getElementById('DeleteFriendModal');
    var modal = bootstrap.Modal.getInstance(myModalEl)
    modal.hide();

    //Locate row & remove / re-draw data table
    let table = $("#FriendListTable").DataTable();
    let row = table.row((idx, data) => data.id === recipientId);
    row.remove().draw();
}

function LoadFriendListTable(friends) {

    var friendListTableJquery = $("#FriendListTable");

    var friendListTable = $("#FriendListTable").DataTable({
        "columns": [
            {
                data : null,
                render: function (data, type, row, meta) {
                    return "<div class='border-primary rounded-2 border p-2'> " +
                                "<div class='col text-center'> " +
                                    "<a class='h5 link-black text-decoration-none' href='/Users/GetProfiles?userId=" + row.id + "'>" + row.displayName + "</a>" +
                                "</div>" +
                            "</div>"
                }
            },
            {
                data : null,
                render: function (data, type, row, meta) {

                    if (IsCurrentUserAdmin() === "True" || friendListTableJquery.attr("profile-user-id") === RetrieveCurrentUserId()) {
                        return "<div class='border-primary rounded-2 border p-2'> " +
                            "<div class='border-end border-primary'> " +
                            "<a class='h5 link-primary remove-friend' role='button' data-bs-toggle='modal' friend-user-displayname='" + row.displayName + "' friend-user-id='" + row.id + "' data-bs-target='#DeleteFriendModal'>Delete</a>"
                        "</div>" +
                            "</div>"
                    }
                    else {
                        return ""
                    }
                }
            }
        ],
        data : friends
    });

}