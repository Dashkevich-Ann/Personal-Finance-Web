(function ($) {
    function Index() {
        var $this = this;
        function initialize() {

            $(".popup").on('click', function (e) {
                modelPopup(this);
            });

            $(document).on("click", "#userEditSubmit", function (e) {
                formSubmit(e);
            })

            $(document).on("click", "#userDeleteConfirm", function (e) {
                submitRemove(e);
            })

            function modelPopup(reff) {
                var url = $(reff).data('url');

                $.get(url).done(function (data) {
                    $('#modal-edit-user').find(".modal-dialog").html(data);
                });
            }

            function formSubmit(e) {
                e.preventDefault();
                var data = $("#editUserForm").serialize();
                var url = $('#editUserForm').attr('action');
                console.log(data);
                $.ajax({
                    type: 'POST',
                    url: url,
                    contentType: 'application/x-www-form-urlencoded; charset=UTF-8', // when we use .serialize() this generates the data in query string format. this needs the default contentType (default content type is: contentType: 'application/x-www-form-urlencoded; charset=UTF-8') so it is optional, you can remove it
                    data: data,
                    success: function (result) {
                        setTimeout(() => {
                            if (result === "Success") {
                                document.location.reload();
                            } else {
                                $('#modal-edit-user').find(".modal-dialog").html(result);
                            }
                        }, 50)
                    },
                    error: function () {
                        alert('Failed to update user');
                    }
                })
            }

            function submitRemove(e) {
                var url = $(e.target).data('url');
                $.ajax({
                    type: 'DELETE',
                    url: url,
                    success: function (result) {
                        setTimeout(() => {
                            if (result === "Success") {
                                document.location.reload();
                            }
                            else
                                alert('Failed to detele user');
                        }, 50)
                    },
                    error: function () {
                        alert('Failed to detele user');
                    }
                })

            }
        }

        $this.init = function () {
            initialize();
        };
    }
    $(function () {
        var self = new Index();
        self.init();
    });
}(jQuery));