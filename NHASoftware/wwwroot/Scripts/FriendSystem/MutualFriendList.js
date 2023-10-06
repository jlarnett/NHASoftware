function LoadMutualFriendListTable(friends) {

    var friendListTable = $("#FriendListTable").DataTable({
        "columns": [
            {
                data : 'displayName',
                render: function (data) {
                    return "<div class='border-primary border'> " +
                                "<div class='col text-center'> " +
                                    "<div class='h1'>" + data + "</div>" +
                                "</div>" +
                            "</div>"
                }
            },
        ],
        data : friends
    });

}