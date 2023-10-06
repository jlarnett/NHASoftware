function LoadFriendListTable(friends) {
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
            {
                data : 'action',
                render: function (data) {

                    return "<div class='border-primary border'> " +
                                "<div class='border-end border-primary'> " +
                                    "<a class='h1 link-danger' role='button'>Delete</a>"
                                "</div>" +
                            "</div>"
                }
            }
        ],
        data : friends
    });

}