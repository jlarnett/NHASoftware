function InitializeForumIndexTables(ForumsKV, adminButtons) {

        var arrayLength = ForumsKV.length;
        var keys = [];


        for (var i = 0; i < arrayLength; i++) {
            if (keys.includes(ForumsKV[i].key)) {

            } 
            else {
                var table = $("#" + ForumsKV[i].key).DataTable();
                keys.push(ForumsKV[i].key);
                table.columns.adjust().draw();
            }
        }

        for (var i = 0; i < arrayLength; i++) {

            var table2 = $("#" + ForumsKV[i].key).DataTable();
            var str = spacetime(ForumsKV[i].value.latestDate);
            var date2 = str.format('{month}-{date-pad}-{year} {time}{am pm}');

            table2.row.add(["<div class='container-fluid bg-gradient bg-secondary modern-forum-post-container mb-2 w-100'> <a role='button' class='text-decoration-none w-100 modern-thread-link' topic-id='" + ForumsKV[i].value.id + "'>" +
                                "<div class='row w-100'>" +
                                    "<div class='col-auto'>" + "<h3 class='h2 text-secondary-emphasis text-decoration-none'>" + ForumsKV[i].value.title + "</h3>" + "</div>" +
                                "</div>" +
                                "<div class='row w-100'>" +
                                    "<p class='h5'>" + ForumsKV[i].value.description + "</p>" +
                                "</div>" +
                                "<div class='row w-100'>" +
                                    "<div class='col row align-content-center m-auto'>" +
                                    CheckForAdminButtons(ForumsKV[i], adminButtons) +
                                        "<div class='col-1 m-auto'>" +
                                            "<input type='image' src='/Images/ThreadIcon.png' class=' img-fluid js-thread-comment' comment-id='" + "commentid" + "'/>" +
                                        "</div>" +
                                        "<div class='col-auto m-auto'>" +
                                            "<h3>" + ForumsKV[i].value.threadCount + "</h3>" +
                                        "</div>" +
                                        "<div class='col-1 m-auto'>" +
                                            "<input type='image' src='/Images/PostIcon.png' class='img-fluid js-thread-comment' comment-id='" + "commentid" + "'/>" +
                                        "</div>" +
                                        "<div class='col-auto m-auto'>" + 
                                        "<h3>" + ForumsKV[i].value.postCount + "</h3>" +
                                    "</div>" +
                                    "<div class='col m-auto'>" +
                                        "<h5 class='text-black h6'>" + "Latest Post: " + date2 + "</h5>" +
                                    "</div>" +
                                "</div>" +
                "</a></div>"]).draw();
                table2.columns.adjust().draw();
        }
}

function CheckForAdminButtons(data, adminButtons) 
{
    if (adminButtons === true) {
        return "<div class='col row m-auto'>" +
        "<div class='col-auto'>" +
                "<button class='btn btn-outline-warning js-edit-topic' topic-id='" +
                    data.value.id +
                "'>Edit Topic</button>" +
            "</div>" +
            "<div class='col-auto'>" +
            "<button class='btn btn-outline-danger js-delete-topic' topic-id='" +
            data.value.id +
            "'>Delete Topic</button>" +
            "</div>" + "</div>";
    } 
    else {
        return "";
    }
}


