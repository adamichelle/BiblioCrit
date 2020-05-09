// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function readURL(input) {
    if (input.files && input.files[0]) {

        var reader = new FileReader();

        reader.onload = function (event) {

            $('.image-upload-wrap').hide();
            $('.file-upload-content').show();

            var image = new Image();
            var imageW = 400;

            image.onload = function () {

                var canvas = document.createElement('canvas');

                canvas.width = image.width;
                canvas.height = image.height;

                var ctx = canvas.getContext('2d');
                ctx.drawImage(image, 0, 0, canvas.width, canvas.height);

                $('#image_input').html(['<img src="', canvas.toDataURL(), '"/>'].join(''));
                var img = $('#image_input img')[0];

                var canvas = document.createElement('canvas');

                $('#image_input img').Jcrop({
                    boxWidth: 550,
                    bgColor: 'black',
                    bgOpacity: .6,
                    setSelect: [0, 0, 400, 400],
                    onChange: imgSelect
                });

                function imgSelect(selection) {
                    canvas.width = 200;
                    canvas.height = 200;
                    canvas.id = "pictureCanvas";

                    var ctx = canvas.getContext('2d');
                    ctx.drawImage(img, selection.x, selection.y, selection.w, selection.h, 0, 0, canvas.width, canvas.height);


                    $('.file-upload-content-crop-preview').show();
                    $('.get-reviews').show();
                    $('#image_output').attr('src', canvas.toDataURL());
                    $('#CroppedImage').val(canvas.toDataURL());
                    //$('#image_source').text(canvas.toDataURL());
                    $('.image-title').html(input.files[0].name);
                }
            }
            image.src = event.target.result;
        }
        reader.readAsDataURL(input.files[0]);

    } else {
        removeUpload();
    }
}

function removeUpload() {
    $('.file-upload-input').replaceWith($('.file-upload-input').clone());
    $('.file-upload-content').hide();
    $('.image-upload-wrap').show();
}
$('.image-upload-wrap').bind('dragover', function () {
    $('.image-upload-wrap').addClass('image-dropping');
});
$('.image-upload-wrap').bind('dragleave', function () {
    $('.image-upload-wrap').removeClass('image-dropping');
});