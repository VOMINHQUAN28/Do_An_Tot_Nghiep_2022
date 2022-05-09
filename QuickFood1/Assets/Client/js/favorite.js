var common = {
    init: function () {
        common.registerEvent();
    },
    registerEvent: function () {
        $('.btnLogin').click(function () {
            PNotify.notice({
                title: 'THÔNG BÁO!!',
                text: 'Bạn vui lòng đăng nhập để thích món ăn này.'
            });
        });

        $('.btnFavorite').click(function () {
            var islike = $(this).children().hasClass('fa-heart-o');
            if (islike) {
                $(this).children().first().removeClass('fa-heart-o');
                $(this).children().first().addClass('fa-heart');
                var count = $(this).children().last().text();
                if (count == "") {
                    $(this).children().last().text("1");
                } else {
                    var Soluong = parseInt(count) + 1;
                    $(this).children().last().text(Soluong);
                }
                $.ajax({
                    data: {
                        User_ID: $(this).data('userid'),
                        Food_ID: $(this).data('foodid'),
                        isLike: true
                    },
                    url: '/Food/AddFavorite',
                    dataType: 'Json',
                    type: 'POST',
                    success: function () { }
                })


            }
            else {
                $(this).children().first().removeClass('fa-heart');
                $(this).children().first().addClass('fa-heart-o');

                var count = $(this).children().last().text();
                var Soluong = parseInt(count) - 1;
                $(this).children().last().text(Soluong);
                if (Soluong == 0) {
                    $(this).children().last().text("");
                } else {
                    $(this).children().last().text(Soluong);
                }
                $.ajax({
                    data: {
                        User_ID: $(this).data('userid'),
                        Food_ID: $(this).data('foodid'),
                        isLike: false
                    },
                    url: '/Food/AddFavorite',
                    dataType: 'Json',
                    type: 'POST',
                    success: function () { }
                })

            }


        });

        $('.btnadd').click(function () {
            var food_id = $(this).data('id');

            $.ajax({
                url: '/order/AddMenu',
                data: {
                    foodId: food_id,
                    quantity: 1,
                },
                type: 'POST',
                dataType: 'json',
                success: function (res) {
                    if (res.status == true) {

                        var count = $('#lblCartCount').text();
                        var Soluong = parseInt(count) + 1;
                        $('#lblCartCount').text(Soluong);

                        const notice = PNotify.success({
                            title: 'THÔNG BÁO!',
                            text: 'Thêm món ăn vào thực đơn thành công. Click để xem thực đơn.'
                        });
                        notice.refs.elem.style.cursor = 'pointer';
                        notice.on('click', e => {
                            window.location.href = "/order"
                        });

                        
                    } else {
                        PNotify.error({
                            title: 'THÔNG BÁO!!',
                            text: 'Đã có lỗi xảy ra, bạn vui lòng thử lại sau.'
                        });
                    }

                }
            });

        });
    }
}
common.init();