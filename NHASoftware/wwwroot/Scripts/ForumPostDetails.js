$(document).ready(function() {
    var comments = JSON.parse('@Html.Raw(Json.Serialize(Model.ForumComments))');

    var arrayLength = comments.length;

    var table = $("#CommentsTable").DataTable({
        "columns": [
            null,
            {"width": "10%"}
        ]
    });

    for (var i = 0; i < arrayLength; i++) {
        var str = spacetime(comments[i].creationDate);
        var date = str.format('{month} {date-pad} {year} {time}{am pm}');
                    

        table.row.add( ["<article class='group-thread group-thread--rootview' data-id='" + comments[i].id + "'>" +
            "<a class='group-thread__link'>" +
            "<div class='group-thread__details'>" +
            "<h3 class='group-thread__heading'>" + comments[i].user.email + "</h3>" +
            "<p class='group-thread__summary'>" + comments[i].commentText + "</p>" +
            "</div>" +
            "<span class='forum-stat forum-stat--thread'>" + 0 + "</span>" +
            "<span class='forum-stat forum-stat--latest'>" + date + "</span>" +
            "</a>" +
            "</article>", "<article class='group-thread group-thread--rootview action-article'>" + "<a class='btn-primary btn-link cmt-action' href='/ForumComments/delete/" + comments[i].id  + "'>Delete</a>" + "<a class='btn-primary btn-link cmt-action' href='/ForumComments/edit/" + comments[i].id  + "'>Edit</a></article>"] ).draw();
    }

    var table2 = $("#PostTable").DataTable({
        searching: false,
        paging: false,
        info: false,

        "columns": [
            null,
            {"width": "10%"}
        ]
    });

    var post = JSON.parse('@Html.Raw(Json.Serialize(Model.ForumPost))');
    console.log(post);

    table2.row.add(["<article class='group-thread group-thread--rootview' data-id='" + 1000 + "'>" +
        "<a class='group-thread__link'>" +

        "<div class='group-thread__details'>" +
        "<h3 class='group-thread__heading'>" + post.user.email + "</h3>" +
        "<h3 class='group-thread__heading'>" + post.title + "</h3>" +
        "<p class='group-thread__summary'>" + post.forumText + "</p>" +

        "</div>" +
        "<span class='forum-stat forum-stat--thread'>" + 0 + "</span>" +
        "<span class='forum-stat forum-stat--latest'>" + date + "</span>" +
        "</a>" +
        "</article>", "<article class='group-thread group-thread--rootview action-article'>" + "<a class='btn-primary btn-link cmt-action' href='/ForumPosts/delete/" + @id  + "'>Delete</a>" + "<a class='btn-primary btn-link cmt-action' href='/ForumPosts/edit/" + @id  + "'>Edit</a></article>"] ).draw();
});