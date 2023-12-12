class ImageLoader {

    static ShouldPostLoadImagesFromDB() {
        //This function is called every time the user scrolls. Goes over each post & checks whether
        //Images should be loaded. If images need to be loaded then it calls the API Retrieval method
        //and appends it to the post. 
        $('.post-container').filter(function() {
            var postIsPartiallyVisibleInView = Utils.isElementInView(this, false);
            var postIsFullyVisibleInView = Utils.isElementInView(this, true);
        
            var postHasImagesAttached = $(this).attr("images-attached");
            var postHasLoadedImagesAlready = $(this).attr("images-loaded");
            var postId = $(this).attr("post-id");
            var uuid = $(this).attr("post-uuid");

            if (postIsPartiallyVisibleInView && postHasImagesAttached !== "False" && postHasLoadedImagesAlready === "false") {
                $(this).attr("images-loaded", "true");
                ImageLoader.RetrieveImagesForPost(postId, uuid);
            }
        });
    }

    static RetrieveImagesForPost(postId, uuid) {
        //Retrieves images for the specified postId & appends it to the post. 
        ContentFeedUtility.AddSpinnerToImageSection(uuid);

        ContentFeedAjaxCalls.RetrieveImagesForPost(postId).then(function (images) {
            ContentFeedUtility.RemoveSpinnerFromImageSection(uuid);
            ImageLoader.LoadImagesToPost(images, uuid);
            ContentFeedUtility.ShowPostImageCarousel(uuid);
        });
    }

    static LoadImagesToPost(images, uuid) {
        //Takes in the list of images retrieved from API Call and creates the HTML for images.
        //The HTML is appended to image section of post. 

        var imageHtml = this.GeneratePostImagesHtmlRedesign(images);
        var indicatorHtml = this.GenerateCarouselIndicatorHtml(images, uuid);

        $("#Image-Carousel-Inner-" + uuid).append(imageHtml);
        $("#Image-Carousel-" + uuid).prepend(indicatorHtml);

        var myCarousel = document.querySelector("#Image-Carousel-" + uuid);
        var carousel = new bootstrap.Carousel(myCarousel, {
            interval: 3000,
        })
    }

    static GenerateCarouselIndicatorHtml(images, uuid) {
        let IndicatorHtml = [];
        let imageCount = 0;

        IndicatorHtml.push('<div id="Image-Carousel-Indicators-', uuid, '" class="carousel-indicators">');

        images.forEach((image) => {
            if (imageCount > 0) {
                IndicatorHtml.push('<button type="button" data-bs-target="#Image-Carousel-', uuid, '" data-bs-slide-to="', imageCount, '" aria-label="Slide ', imageCount + 1, '"></button>');
            }
            else {
                IndicatorHtml.push('<button type="button" data-bs-target="#Image-Carousel-', uuid, '" data-bs-slide-to="', imageCount, '" class="active" aria-current="true" aria-label="Slide 1"></button>');
            }

            imageCount += 1;
        });

        IndicatorHtml.push('</div>');
        return IndicatorHtml.join('');
    }

    static GeneratePostImagesHtmlRedesign(images) {
        //Takes in the list of images retrieved from API Call and creates the HTML for images.
        //The HTML is appended to image section of post. 
        let postImageHtml = [];
        let imageCount = 0;

        images.forEach((image) => {
            if (imageCount > 0) {
                postImageHtml.push('<div class="carousel-item"><img class="d-block w-100" src="', image, '" alt="Post Image"/></div>');
            }
            else {
                postImageHtml.push('<div class="carousel-item active"><img class="d-block w-100" src="', image, '" alt="Post Image"/></div>');
            }

            imageCount += 1;
        });

        return postImageHtml.join('');
    }
}