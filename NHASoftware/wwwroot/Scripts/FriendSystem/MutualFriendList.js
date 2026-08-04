function LoadMutualFriendListTable(friends) {
    //Loads the mutual friend list
    var friendListTable = $("#FriendListTable").DataTable({
        autoWidth: false,
        responsive: true,
        lengthChange: false,
        pageLength: 10,
        language: {
            search: "",
            searchPlaceholder: "Search mutual friends...",
            emptyTable: "No mutual friends found."
        },
        "columns": [
            {
                data : null,
                render: function (data, type, row, meta) {
                    var profilePicturePath = row.profilePicturePath ? row.profilePicturePath : "DefaultProfilePicture.png";

                    return "<div class='connection-cell-card'>" +
                                "<img class='connection-user-avatar' src='/ProfilePictures/" + profilePicturePath + "' alt='" + row.displayName + " profile picture' />" +
                                "<div class='connection-user-meta'>" +
                                    "<a class='connection-user-link' href='/Users/GetProfiles?userId=" + row.id + "' role='button'>" + row.displayName + "</a>" +
                                    "<span class='connection-user-caption'>Mutual friend</span>" +
                                "</div>" +
                            "</div>";
                }
            },
        ],
        data : friends
    });

}