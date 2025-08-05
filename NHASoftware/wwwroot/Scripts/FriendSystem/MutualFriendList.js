function LoadMutualFriendListTable(friends) {
    //Loads the mutual friend list
    var friendListTable = $("#FriendListTable").DataTable({
        "columns": [
            {
                data : null,
                render: function (data, type, row, meta) {
                    return "<div class='border-primary border rounded-2'> " +
                                "<div class='col text-center'> " +
                                    "<a class='h5 link-dark text-decoration-none' href='/Users/GetProfiles?userId=" + row.id + "' role='button'>" + row.displayName + "</a>" +
                                "</div>" +
                            "</div>"
                }
            },
        ],
        data : friends
    });

}