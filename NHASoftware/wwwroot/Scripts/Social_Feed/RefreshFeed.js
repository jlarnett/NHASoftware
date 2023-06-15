$(document).ready(function () {

    $('#HomeContentFeed').on('scroll', function() {
        //Checks the home pages content feed scrollbar. Whenever scrollbar is bottom position retrieve more posts & load
        //into the home page feed.
        if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
            console.log("Loading more post.......................");
            OptimizedFeedLoad();
        }
    });

    console.log("Loading Profile Posts.....................");
    LoadFeedWithProfilePost($('#ContentFeed').attr("profile-user-id"));
});

function RetrieveMorePosts() {
    ///AJAX CALL TO api/posts webapi. Returns async result of posts
    return $.get("api/posts", function (data) {
    });
}

function RetrievePostComments(postId) {
    ///AJAX CALL TO api/posts/findchildrenposts webapi endpoint. 
    //Returns async result all 'Comments' => Comments are just post with fatherPostId populated.
        return $.get("/api/posts/findchildrenposts/" + postId, function(data) {
    });
}

function RetrieveAllPostForUser(userId) {
    //Calls the Post WebAPI & gets all post created under supplied userId
        return $.get("/api/posts/GetSocialPostForUserId/" + userId, function(data) {
    });
}

function LoadPostComments(id, uuid) {
    //Generates & loads comments for a post id. Receives uuid which is identifier to know which post to load comments for on home page. 

    var commentHtml = [];

    RetrievePostComments(id).then(function (data) {
        for (var i = 0; i < data.length; i++) {
            commentHtml.push('<li>',
                '<div class="comment-container">',
                '<div class="comment-profile-picture">',
                '<img class="comment-profile-picture-item" src="/ProfilePictures/', data[i].user.profilePicturePath, '" />',
                '</div>',
                '<div class="comment-details">',
                '<div>',
                '<a class="feed-profile-link Post-Header-Item" userId="', data[i].user.id, '">', data[i].user.displayName, '</a>',
                '</div>',
                data[i].summary,
                GeneratePostLikeSection(data[i]),
                '</div>',
                '</div',
                '</li>');

            var commentListElement = $("ul[unique-comment-list$=" + uuid + "]");

            commentListElement.empty();
            commentListElement.html(commentHtml.join(''));
        }
    });
}

function TryInsertPostCommentTextbox(postId, uuid) {
//Tries to insert textbox below comment section. Checks if user is logged in & returns the textbox html if logged in.
    var Html = [];
    var isLoggedIn = CheckUserSessionIsActive();

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

    var currentDate = spacetime.now();
    var postCreationDate = spacetime(post.creationDate);
    var postDateDifferenceInSeconds = postCreationDate.diff(currentDate, 'second');

    postHtml.push('<div class="MainFeedPostContainer">',
                '<div>',
                    '<div class="Main-Post-User-Profile">',
                        '<img src="ProfilePictures/', post.user.profilePicturePath, '"class="Main-Post-User-Profile-Picture">',
                    '</div>',
                    '<div class="Main-Post">',
                        '<div class="Main-Post-Top-Header Main-Post-Section">',
                            '<a class="feed-profile-link Post-Header-Item" userId="', post.user.id, '">', post.user.displayName, '</a>',
                            '<a class="Post-Header-Item">-</a>',
                            '<a class="Post-Header-Item">', GetTimeShortHandString(postDateDifferenceInSeconds) ,'</a>',
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

    var likeSrcImage = (post.userLikedPost ? "/images/facebook-like-filled.png" : "/images/facebook-like.png");
    var dislikeSrcImage = (post.userDislikedPost ? "/images/dislike-filled.png" : "/images/dislike.png");

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


//Feed bootstrap redesign section
function LoadFeedWithProfilePost(userId) {
    //Loads the id #ContentFeed with all posts created by user. Calls Post WebAPI
    RetrieveAllPostForUser(userId).then(function (posts) {
        var feedHtml = [];

        for (var i = 0; i < posts.length; i++) {
            feedHtml.push(GeneratePostRedesign(posts[i]));
        }

        var builtFeedHtml = feedHtml.join("");
        $("#ContentFeed").empty();
        $("#ContentFeed").append(builtFeedHtml);
        RebuildFeedTextboxes();
    });
}

function GeneratePostRedesign(post) {
    //This method receives post objects & generates the standardized post's html strings
    let uuid = self.crypto.randomUUID();
    var postHtml = [];

    var currentDate = spacetime.now();
    var postCreationDate = spacetime(post.creationDate);
    var postDateDifferenceInSeconds = postCreationDate.diff(currentDate, 'second');

    postHtml.push('<div class="container-fluid border border-primary mt-4" post-delete-id="', post.id, '">',
            '<div class="row align-items-center">',
                '<!--User Profile image & display name-->',
                '<div class="col-1 px-0">',
                    '<img src="/ProfilePictures/', post.user.profilePicturePath, '" class="img-fluid"/>',
                '</div>',
                '<div class="col-auto border-primary border-start">',
                    '<a class="h3 text-decoration-none profile-link" role="button" userId="', post.user.id, '">', post.user.displayName, '</a>',
                '</div>',
                '<div class="col-auto">',
                    '<a class="h3 text-decoration-none" role="button">-</a>',
                '</div>',
                '<div class="col-2">',
                    '<a class="h3 text-decoration-none" role="button">', GetTimeShortHandString(postDateDifferenceInSeconds), '</a>',
                '</div>',
                '<div class="col-6">',
                '</div>',
                '<!--Post Action dropdown-->',
                '<div class="col-auto">',
                    GeneratePostActionButton(post),
                '</div>',
            '</div>',
            '<div class="row">',
                '<div class="col-sm-1 px-0"></div>',
                '<div class="col border-bottom border-primary">',
                    '<div class="h4 text-primary">Summary</div>',
                '</div>',
            '</div>',
            '<!--Posts summary section. Text content is posted here-->',
            '<div class="row">',
                '<div class="col-sm-1 px-0"></div>',
                '<p class="text-black col text-white border-bottom border-start border-primary">', post.summary, '</p>',
            '</div>',
            '<!--Posts Likes & Comment Show section-->',
            '<div class="row align-items-center m-2">',
                '<div class="col-auto">',
                    '<a class="link-light hide-comments" unique-post-id="', uuid, '" post-id="', post.id, '" role="button">Show Comments</a>',
                '</div>',
                '<div class="col-5"></div>',
                GenerateSocialLikeSectionRedesign(post),
            '</div>',
            '<div unique-comment-section="', uuid, '" style="display: none">',
                    '<ul unique-comment-list="', uuid, '" class="list-unstyled">',
                    '</ul>',
                    InsertCommentTextboxRedesignHtml(post.id, uuid),
            '</div>',
        '</div>');

    return postHtml.join('');
}

function GeneratePostActionButton(post) {
    //Generates the post action dropdown button. Available actions are determined & added to the dropdown before returning
    actionButtonHtml = [];

    actionButtonHtml.push('<div class="dropstart">',
                            '<button class="btn btn-dark dropdown-toggle" type="button" id="dropstartMenuButton" data-bs-toggle="dropdown" aria-expanded="false">',
                                '<img src="/Images/options.png" class="img-fluid"/>',
                            '</button>',
                            '<ul class="dropdown-menu" aria-labelledby="dropstartMenuButton" style="">',
                                '<li><h6 class="dropdown-header">Actions</h6></li>',
                                '<li><hr class="dropdown-divider"></li>',
                                GenerateHideActionButtonLink(post),
                                GenerateDeleteActionButtonLink(post),
                                GenerateReportActionButtonLink(post),
                            '</ul>',
                        '</div>');

    return actionButtonHtml.join('');
}

function GenerateDeleteActionButtonLink(post) {
    //Returns a string of the Delete Post Action Button HTML as long as user is admin or the owner of the post. 
    var isCommentBool = IsComment(post);

    if (post.user.id === RetrieveCurrentUserId() || IsCurrentUserAdmin() === "True") {
        return '<li><a class="dropdown-item delete-post-link" post-id="' + post.id +'" is-comment="' + isCommentBool +'">Delete Post</a></li>';
    }
}

function GenerateHideActionButtonLink(post) {
    //Returns a string of the Hide Post Action Button HTML as long as user is admin or the owner of the post. 
    var isCommentBool = IsComment(post);
    if (post.user.id === RetrieveCurrentUserId() || IsCurrentUserAdmin() === "True") {
        return '<li><a class="dropdown-item hide-post-link" post-id="' + post.id + '" is-comment="' + isCommentBool + '">Hide Post</a></li>';
    }
}

function GenerateReportActionButtonLink(post) {
    //Returns a string of the Report Post Action Button HTML as long as user is logged in to site. 
    var isCommentBool = IsComment(post);
    if (CheckUserSessionIsActive() === "True") {
            return '<li><a class="dropdown-item report-post-link" post-id="' + post.id + '" is-comment="' + isCommentBool + '">Report Post</a></li>';
    }
}

function IsComment(post) {
    //Returns bool whether the supplied post is a comment or a primary post. 
    if (post.parentPostId === null) { 
        return false;
    }
    return true;
}

function LoadCommentsRedesign(id, uuid) {
    //Loads comments for post using bootstrap redesign. Takes the post-id & uuid to determine which comment section to load
    //Called by Hide Comments js script. 
    var commentHtml = [];
    var currentDate = spacetime.now();

    RetrievePostComments(id).then(function (data) {
        for (var i = 0; i < data.length; i++) {

            var postCreationDate = spacetime(data[i].creationDate);
            var postDateDifferenceInSeconds = postCreationDate.diff(currentDate, 'second');

            commentHtml.push('<li comment-delete-id="', data[i].id ,'">',
                '<div>',
                    '<div class="row align-items-center">',
                        '<div class="col-1"></div>',
                        '<div class="col-1 px-0">',
                            '<div class="row align">',
                                '<img class="img-fluid col-6 m-auto" src="/ProfilePictures/', data[i].user.profilePicturePath, '" />', 
                            '</div>',
                        '</div>',
                        '<div class="col-auto border-primary border-start">',
                            '<a class="profile-link ms-4 h6 text-decoration-none" role="button" userId="', data[i].user.id, '">', data[i].user.displayName, '</a>',
                        '</div>',
                        '<div class="col-auto">',
                            '<a class="text-decoration-none ms-2 h6">', "-", '</a>',
                        '</div>',
                        '<div class="col-1">',
                            '<a class="text-decoration-none ms-2 h6">', GetTimeShortHandString(postDateDifferenceInSeconds), '</a>',
                        '</div>',
                        '<div class="col-4"></div>',
                        '<div class="col-auto">',
                            GeneratePostActionButton(data[i]),
                        '</div>',
                    '</div>',
                    '<div class="row align-items-center">',
                        '<div class="col-2"></div>',
                        '<div class="col-6 align-middle ms-4 p-2 border-primary border-bottom">', data[i].summary, '</div>',
                    '</div>',
                    GenerateSocialLikeSectionRedesign(data[i]),
                    '<hr/>',
                '</div>',
                '</li>');

            var commentListElement = $("ul[unique-comment-list$=" + uuid + "]");

            commentListElement.empty();
            commentListElement.html(commentHtml.join(''));
        }
    });
}
function GenerateSocialLikeSectionRedesign(post) {
    //Takes the user post & generates the like section html. Determines Initial Like Icon states using value returned in postDto object
    var likeSectionHtml = [];

    var likeSrcImage = (post.userLikedPost ? "/images/facebook-like-filled.png" : "/images/facebook-like.png");
    var dislikeSrcImage = (post.userDislikedPost ? "/images/dislike-filled.png" : "/images/dislike.png");

    likeSectionHtml.push('<div class="row mt-2">',
        '<div class="col-5"></div>',
        '<div class="col-1">',
            '<div class="row">',
                '<div class="col-auto me-2" like-counter-post-id="', post.id, '">', post.likeCount ,'</div>',
                '<div class="col-4">',
                    '<img class="post-like img-fluid" role="button" post-id="', post.id,'" src="', likeSrcImage , '" />',
                '</div>',
            '</div>',
        '</div>',
        '<div class="col-1">',
            '<div class="row">',
                '<div class="col-auto me-2" dislike-counter-post-id="', post.id,'">', post.dislikeCount ,'</div>',
                '<div class="col-4">',
                    '<img class="post-dislike img-fluid" role="button" post-id="', post.id,'" src="', dislikeSrcImage , '" />',
                '</div>',
            '</div>',
        '</div>',
    '</div>');

    return likeSectionHtml.join('');
}

function InsertCommentTextboxRedesignHtml(postId, uuid) {
//Tries to insert textbox below comment section. Checks if user is logged in & returns the textbox html if logged in.
    var Html = [];
    var isLoggedIn = CheckUserSessionIsActive();

    if (isLoggedIn === 'True') {
        Html.push('<div class="row mb-2">',
            '<div class="col-2"></div>',
            '<div class="col-5">',
                '<div class="summernote-comments" post-id="', postId, '" unique-comment-identifier="', uuid  ,'">',
                '</div>',
            '</div>',
            '<button class="btn-dark comment-send-btn col-1" unique-identifier="', uuid ,'">Reply</button>',
            '</div>', '<span unique-error-identifier="', uuid, '" style="display: none; color: red">Error Submitting Post.....</span>');
    }
    else {
        Html.push('');
    }

    return Html.join('');
}


