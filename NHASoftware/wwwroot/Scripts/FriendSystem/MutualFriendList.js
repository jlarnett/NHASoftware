function LoadMutualFriendListTable(friends) {
    //Loads the mutual friend list
    var friendListTable = $("#FriendListTable").DataTable({
        "columns": [
            {
                data : null,
                render: function (data, type, row, meta) {
                    return "<div class='border-primary border'> " +
                                "<div class='col text-center'> " +
                                    "<a class='h1 link-secondary text-decoration-none' href='/Users/GetProfiles?userId=" + row.id + "' role='button'>" + row.displayName + "</a>" +
                                "</div>" +
                            "</div>"
                }
            },
        ],
        data : friends
    });

}