    bindInvalidHandler: function () {
        if ($("form").find(".validation-result").size() == 0) {
            var self = this;
            $("form").unbind("invalid-form")
                .bind("invalid-form", function () {
                    var messages = new Array();
                    $.each($("form").validate().invalid, function (index, value) {
                        messages.push(value);
                    });
                    messages = self.unique(messages).join("<br /><br />");
                    $.messageHandler.popup(messages);
                });
        } else {
            $("form").unbind("invalid-form")
                .bind("invalid-form", function () {
                    //$('.validation-banner').show();
                });
            // PK TODO: Crude hack to remove validation text so that it doesn't display when using ticks and crosses
            $('[data-val]').blur(function () {
                $('.validation-result span').hide();
            });
            $("form").submit(function () {
                $('.validation-result span').hide();
                $.messageHandler.closeStickyTooltip();
                $(".field-validation-error:first").each(function () {
                    $(this).messageHandler({
                        type: "tooltip",
                        sticky: true,
                        message: this.textContent,
                        position: 'right'
                    });
                });
            });
        }
    },