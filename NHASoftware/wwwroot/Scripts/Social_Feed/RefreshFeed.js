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
            commentHtml.push('<li>',
                '<div class="comment-container">',
                '<div class="comment-profile-picture">',
                '<img class="comment-profile-picture-item" src="ProfilePictures/', data[i].user.profilePicturePath, '" />',
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
                            '<a class="Post-Header-Item">', ReturnAgeString(postDateDifferenceInSeconds) ,'</a>',
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

    var likeSrcImage = (post.userLikedPost ? "images/facebook-like-filled.png" : "images/facebook-like.png");
    var dislikeSrcImage = (post.userDislikedPost ? "images/dislike-filled.png" : "images/dislike.png");

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

function ReturnAgeString(ageInSeconds) {
    //Returns a rounded shorthand string for the age of post. E.G 2H A
    var ageInMinutes = RoundNumber(ageInSeconds / 60);
    var ageInHours = RoundNumber(ageInMinutes / 60);
    var ageInDays = RoundNumber(ageInHours / 24);
    var ageInYears = RoundNumber(ageInDays / 365);

    var outputString;


    if (ageInSeconds < 60 && ageInSeconds >= 1) {
        if (ageInSeconds > 1) {
            outputString = " seconds ago";
        }
        else {
            outputString = " second ago";
        }

        return ageInSeconds + outputString;
    }
    if (ageInMinutes < 60 && ageInMinutes >= 1) {

        if (ageInMinutes > 1) {
            outputString = " minutes ago";
        }
        else {
            outputString = " minute ago";
        }

        return ageInMinutes + outputString;
    }

    if (ageInHours < 24 && ageInHours >= 1) {

        if (ageInHours > 1) {
            outputString = " hours ago";
        }
        else {
            outputString = " hour ago";
        }

        return ageInHours + outputString;
    }

    if (ageInDays < 365 && ageInDays >= 1) {

        if (ageInDays > 1) {
            outputString = " days ago";
        }
        else {
            outputString = " day ago";
        }

        return ageInDays + outputString;
    }

    if (ageInYears >= 1) {

        if (ageInYears > 1) {
            outputString = " years ago";
        }
        else {
            outputString = " year ago";
        }

        return ageInYears + outputString;
    }
}

function RoundNumber(number) {
    //Returns rounded number of parameter without excess 
    return Math.round(number);
}

