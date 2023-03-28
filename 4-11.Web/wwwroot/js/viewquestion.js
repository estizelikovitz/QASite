$(() => {
    $("#likes-question").on('click', function () {
        const id = $("#likes-question").data('question-id');
        $.post('/home/addlike', { id }, function () {

            $("#likes-question").addClass('text-danger');
            $("#likes-question").unbind('click');

        });
    });
    setInterval(() => {
        const id = $("#q-id").val();
        $.get(`/home/getlikes`, { id }, function (likes) {
            $("#likes-count").text(likes);
            
        })
    }, 500)

});



//$(() => {
//    $('#like-button').on('click', function () {
//        const id = $(this).data('id');

//        $.post(`/home/likes`, { id }, function () {
//            $("#like-button").prop('disabled', true);
//        })
//    });

//    setInterval(() => {
//        const id = $("#image-id").val();
//        $.get(`/home/getlikes`, { id }, function (likes) {
//            $("#likes-count").text(likes);
//        })
//    }, 500)
//});