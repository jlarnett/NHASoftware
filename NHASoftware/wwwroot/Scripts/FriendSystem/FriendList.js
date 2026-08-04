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
        autoWidth: false,
        responsive: true,
        lengthChange: false,
        pageLength: 10,
        language: {
            search: "",
            searchPlaceholder: "Search friends...",
            emptyTable: "No friends found."
        },
        columnDefs: [
            {
                targets: 1,
                orderable: false,
                searchable: false,
                width: "1%"
            }
        ],
        "columns": [
            {
                data : null,
                render: function (data, type, row, meta) {
                    var profilePicturePath = row.profilePicturePath ? row.profilePicturePath : "DefaultProfilePicture.png";

                    return "<div class='connection-cell-card'>" +
                                "<img class='connection-user-avatar' src='/ProfilePictures/" + profilePicturePath + "' alt='" + row.displayName + " profile picture' />" +
                                "<div class='connection-user-meta'>" +
                                    "<a class='connection-user-link' href='/Users/GetProfiles?userId=" + row.id + "'>" + row.displayName + "</a>" +
                                    "<span class='connection-user-caption'>View profile</span>" +
                                "</div>" +
                            "</div>";
                }
            },
            {
                data : null,
                render: function (data, type, row, meta) {

                    if (IsCurrentUserAdmin() === "True" || friendListTableJquery.attr("profile-user-id") === RetrieveCurrentUserId()) {
                        return "<div class='connection-action-card'>" +
                                    "<a class='btn btn-outline-danger btn-sm connection-action-link remove-friend' role='button' data-bs-toggle='modal' friend-user-displayname='" + row.displayName + "' friend-user-id='" + row.id + "' data-bs-target='#DeleteFriendModal'>Remove</a>" +
                                "</div>";
                    }
                    else {
                        return "<div class='connection-action-card connection-action-card-empty'><span class='connection-user-caption'>No actions</span></div>";
                    }
                }
            }
        ],
        data : friends
    });

}