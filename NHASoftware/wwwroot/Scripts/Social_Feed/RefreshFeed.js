$(document).ready(function () {

    $('#HomeContentFeed').on('scroll', function() {
        //Checks the home pages content feed scrollbar. Whenever scrollbar is bottom position retrieve more posts & load
        //into the home page feed.
        if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
            console.log("Loading more post.......................");
            OptimizedFeedLoad();
        }
    });
});

function RetrieveMorePosts() {
    ///AJAX CALL TO api/posts webapi. Returns async result of posts

    return $.get("api/posts", function (data) {
    });
}

function RetrievePostComments(postId) {
    ///AJAX CALL TO api/posts/findchildrenposts webapi endpoint. 
    //Returns async result all 'Comments' => Comments are just post with fatherPostId populated.

    return $.get("api/posts/findchildrenposts/" + postId, function(data) {
    });
}

function LoadPostComments(id, uuid) {
    //Generates & loads comments for a post id. Receives uuid which is identifier to know which post to load comments for on home page. 

    var commentHtml = [];

    RetrievePostComments(id).then(function (data) {
        for (var i = 0; i < data.length; i++) {
            console.log("Comment Like Count = " + data[i].likeCount)
            commentHtml.push('<li>',
                '<div class="comment-container">',
                '<div class="comment-profile-picture">',
                '<img class="comment-profile-picture-item" src="ProfilePictures/', data[i].user.profilePicturePath, '" />',
                '</div>',
                '<div class="comment-details">',
                '<h5>', data[i].user.displayName, '</h5>',
                data[i].summary,
                GeneratePostLikeSection(data[i]),
                '</div>',
                '</div',
                '</li>');

            $("ul[unique-comment-list$=" + uuid + "]").empty();
            $("ul[unique-comment-list$=" + uuid + "]").html(commentHtml.join(''));
        }
    });
}

function CheckUserSessionActiveFeedAtribute() {
//Checks the homeContentFeed div's user-session-active attribute. Used by posts & comments to check what to render
    return $("#HomeContentFeed").attr("user-session-active");
}


function TryInsertPostCommentTextbox(postId, uuid) {
//Tries to insert textbox below comment section. Checks if user is logged in & returns the textbox html if logged in.
    var Html = [];
    var isLoggedIn = CheckUserSessionActiveFeedAtribute();
    console.log("COMMENT UUID IS = " + uuid);

    if (isLoggedIn === 'True') {
        Html.push('<div class="comment-textbox-area">',
            '<div class="summernote-comments" post-id="', postId, '" unique-comment-identifier="', uuid  ,'">',
            '</div>',
            '<button class="btn-dark comment-send-btn" unique-identifier="', uuid ,'">Reply</button>',
            '</div>', '<span unique-error-identifier="', uuid, '" style="display: none; color: red">Error Submitting Post.....</span>');
    }
    else {
        Html.push('');
    }

    return Html.join('');
}


function OptimizedFeedLoad() {
    //Cleaner more optimized feed load. Receives post from Post API & Adds Them to the Home Content Feed.
    RetrieveMorePosts().then(function (posts) {
        var postHtml = [];

        for (var i = 0; i < posts.length; i++) {
            postHtml.push(GeneratePostHtml(posts[i]));
        }

        var builtPostHtml = postHtml.join("");
        $("#HomeContentFeed").append(builtPostHtml);
        RebuildFeedTextboxes();
    });
}

function GeneratePostHtml(post) {
    //This method receives home page post objects & generates the post's html strings
    let uuid = self.crypto.randomUUID();
    var postHtml = [];

    console.log("Post like count = ", post.likeCount);
    postHtml.push('<div class="MainFeedPostContainer">',
                '<div>',
                    '<div class="Main-Post-User-Profile">',
                        '<img src="ProfilePictures/', post.user.profilePicturePath, '"class="Main-Post-User-Profile-Picture">',
                    '</div>',
                    '<div class="Main-Post">',
                        '<div class="Main-Post-Top-Header Main-Post-Section">',
                            '<a class="link-primary feed-profile-link Post-Header-Item" userId="', post.user.id, '">', post.user.displayName, '</a>',
                            '<a class="link-primary Post-Header-Item">-</a>',
                            '<a class="link-primary Post-Header-Item">2h</a>',
                        '</div>',
                        '<div class="Main-Post-Summary Main-Post-Section">',
                            '<p>', post.summary, '</p>',
                        '</div>', 
                        GeneratePostLikeSection(post),
                        '<div class="Main-Post-Comments Main-Post-Section">',
                            '<div class="comments-header-row">',
                                '<a class="link-light hide-comments" unique-post-id="', uuid,'" post-id="', post.id,'">Show Comments</a>',
                            '</div>',
                            '<div>',
                                '<div data-post-id="', post.id, '" unique-post-comment-section-id="', uuid, '" style="display: none;">', 
                                    '<ul unique-comment-list="',uuid, '">',
                                    '</ul>',
                                    TryInsertPostCommentTextbox(post.id, uuid),
                                '</div>',
                            '</div>',
                        '</div>',
                    '</div>',
                '</div>',
                '<br />',
            '</div>');

    return postHtml.join('');
}

function GeneratePostLikeSection(post) {
    //Takes the user post & generates the like section html. Determines Initial Like Icon states using value returned in postDto object
    var likeSectionHtml = [];

    var likeSrcImage = (post.userLikedPost ? "Images/Facebook-Like-Filled.png" : "Images/Facebook-Like.png");
    var dislikeSrcImage = (post.userdisLikedPost ? "Images/dislike-Filled.png" : "Images/dislike.png");

    likeSectionHtml.push('<div class="Main-Post-Bottom-Actionbar Main-Post-Section">',
        '<div class="Post-Like-Group">',
            '<div class="Like-Group-Item">', post.likeCount ,'</div>',
            '<img class="Post-Like-Icon post-like" post-id="', post.id,'" src="', likeSrcImage , '" />',
        '</div>',
        '<div class="Post-Like-Group">',
            '<div class="Like-Group-Item">', post.dislikeCount, '</div>',
            '<img class="Post-Like-Icon post-dislike" post-id="', post.id,'" src="', dislikeSrcImage ,'" />',
        '</div>',
    '</div>');

    return likeSectionHtml.join('');
}

