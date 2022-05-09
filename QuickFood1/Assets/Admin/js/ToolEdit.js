var tool = {
    init: function () {
        tool.regEvent();
    },
    regEvent: function () {
        //Xóa khách hàng        
        $('.btn-Del-User').off('click').on('click', function (e) {
            var conf = confirm("Bạn có muốn xóa khách hàng này??");
            if (conf == true) {
                $.ajax({
                    data: { id: $(this).data('id') },
                    url: '/Admin/OrderTable/Delete',
                    dataType: 'Json',
                    type: 'POST',
                    success: function (res) {
                        if (res.status == true) {
                            window.location.href = "/Admin/OrderTable/Index";
                        }
                        else {
                            window.location.href = "/Admin/OrderTable/Index";
                        }
                    }
                });
            }

        });

        //Xóa món ăn
        $('.btn-Del-Food').off('click').on('click', function (e) {
            var conf = confirm("Bạn có muốn xóa món ăn này??");
            if (conf == true) {
                $.ajax({
                    data: { id: $(this).data('id') },
                    url: '/Admin/Fooder/Delete',
                    dataType: 'Json',
                    type: 'POST',
                    success: function (res) {
                        if (res.status == true) {
                            window.location.href = "/Admin/Fooder/Index";
                        }
                        else {
                            window.location.href = "/Admin/Fooder/Index";
                        }
                    }
                });
            }
        });

        //Xóa slide món ăn
        $('.btn-Del-Slide').off('click').on('click', function (e) {
            var conf = confirm("Bạn có muốn xóa slide này??");
            if (conf == true) {
                $.ajax({
                    data: { id: $(this).data('id') },
                    url: '/Admin/Slide/Delete',
                    dataType: 'Json',
                    type: 'POST',
                    success: function (res) {
                        if (res.status == true) {
                            window.location.href = "/Admin/Slide/Index";
                        }
                        else {
                            window.location.href = "/Admin/Slide/Index";
                        }
                    }
                });
            }
        });

        //Xóa món ăn trong thực đơn
        $('.btn-Del-OrderDetail').off('click').on('click', function (e) {
            var idname = $(this).data('order');            
            var conf = confirm("Bạn có muốn xóa món ăn này khỏi thực đơn này??");
            if (conf == true) {
                $.ajax({
                    data: { id: $(this).data('id') },
                    url: '/Admin/OrderDetail/DeleteInCus',
                    dataType: 'Json',
                    type: 'POST',
                    success: function (res) {
                        if (res.status == true) {
                            window.location.href = "/Admin/OrderDetail/Index?orderTB_id=" + idname;
                        }
                        else {
                            window.location.href = "/Admin/Slide/Index?orderTB_id=" + idname;
                        }
                    }
                });
            }
        });

    }
}

tool.init();


$(function () {
    //nếu không có thao tác gì thì ẩn đi
    $('#AlertBox').removeClass('hide');

    //Sau khi hiển thị lên thì delay 1s và cuộn lên trên sử dụng slideup
    $('#AlertBox').delay(5000).slideUp(500);
});