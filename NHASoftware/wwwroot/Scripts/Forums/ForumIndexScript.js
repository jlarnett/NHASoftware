function InitializeForumIndexTables(ForumsKV, adminButtons) {

        var arrayLength = ForumsKV.length;
        var keys = [];


        for (var i = 0; i < arrayLength; i++) {
            if (keys.includes(ForumsKV[i].key)) {

            } 
            else {
                var table = $("#" + ForumsKV[i].key).DataTable();
                keys.push(ForumsKV[i].key);
            }
        }

        for (var i = 0; i < arrayLength; i++) {

            var table2 = $("#" + ForumsKV[i].key).DataTable();
            var str = spacetime(ForumsKV[i].value.latestDate);
            var date2 = str.format('{month}-{date-pad}-{year} {time}{am pm}');

            table2.row.add(["<div class='modern-forum-post-container'> <a class='modern-thread-link' topic-id='" + ForumsKV[i].value.id + "'>" +
                                "<div class='modern-forum-post-row-container'>" +
                                    "<div class='modern-forum-topic-title'>" + "<h3 class='modern-post-text'>" + ForumsKV[i].value.title + "</h3>" + "</div>" +
                                "</div>" +
                                "<div>" +
                                    "<p class='modern-formum-topic-summary'>" + ForumsKV[i].value.description + "</p>" +
                                "</div>" +
                                "<div class='modern-post-actions'>" +
                                    CheckForAdminButtons(ForumsKV[i], adminButtons) +
                                    "<div class='modern-post-actions-like modern-post-action'>" +
                                        "<input type='image' src='/Images/ThreadIcon.png' class='modern-threads-picture js-thread-comment' comment-id='" + "commentid" + "'/>" +
                                        "<h3>" + ForumsKV[i].value.threadCount + "</h3>" +
                                    "</div>" +
                                    "<div class='modern-post-actions-like modern-post-action'>" +
                                        "<input type='image' src='/Images/PostIcon.png' class='modern-threads-picture js-thread-comment' comment-id='" + "commentid" + "'/>" +
                                        "<h3>" + ForumsKV[i].value.postCount + "</h3>" +
                                    "</div>" +
                                    "<div class='modern-post-action'>" +
                                        "<h5>" + "Latest Post: " + date2 + "</h5>" +
                                    "</div>" +
                                "</div>" +
                "</a></div>"]).draw();
        }
}

function CheckForAdminButtons(data, adminButtons) 
{
    if (adminButtons === true) {
        return "<div class='modern-post-action'>" +
            "<button class='btn-dark js-edit-topic' topic-id='" +
            data.value.id +
            "'>Edit Topic</button>" +
            "</div>" +
            "<div class='modern-post-action'>" +
            "<button class='btn-dark js-delete-topic' topic-id='" +
            data.value.id +
            "'>Delete Topic</button>" +
            "</div>";
    } 
    else {
        return "";
    }
}


