/*global $, ApplicationPath, document, vipInfoModel, ko, alert, arraysToObject*/

/*jslint eqeq: true */

define(['vipInfoData', 'knockout', 'jquery', 'knockout.viewmodel', 'knockout.util', 'knockout.dirtyFlag',
        'knockout.validation', 'knockout.command'], function (data, ko, $) {

    // Opens confirm status dialog and returns deferred object
    confirmStatusChange = (function ($) {
        var $dialog = $("#WarningDiv");
        $dialog.dialog({
            height: "auto",
            width: 350,
            modal: true,
            position: 'center',
            autoOpen: false,
            title: 'Confirm Change of Edge Status',
            overlay: { opacity: 0.6, background: 'black' }
        });
        return function () {
            var deferred = $.Deferred();
            $dialog.dialog('option', 'buttons',
                {
                    "Confirm": function () {
                        deferred.resolve();
                        $(this).dialog("close");
                    },
                    "Cancel": function () {
                        deferred.reject();
                        $(this).dialog("close");
                    }
                });
            $dialog.dialog("open");
            return deferred.promise();
        };
    }($));

    //ko.viewmodel.options = { logging: true };
    console.log(data.constants);

    var root = data.model;
    root.constants = data.constants;

    var viewModel = ko.viewmodel.fromModel(root, {
        exclude: [
        ],
        append: [
            "{root}.constants"
        ],
        extend: {
            "{root}.constants": function (consts) {
                alert(consts);
            },
            "{root}": { 
                map: function (root) {

                    var constants = root.constants;

                    // Tier
                    root.tierName = ko.computed(function () {
                        return constants.TierNames[root.Tier()];
                    });
                    root.isProTier = ko.computed(function () {
                        return root.Tier() === constants.TierEnum.Pro;
                    });
                    root.tierIsNotNone = ko.computed(function () {
                        return root.Tier() !== constants.TierEnum.None;
                    });

                    // Pro Type
                    root.proTypeName = ko.computed(function () {
                        return constants.ProTypeNames[root.ProType()];
                    });

                    // Edge status description
                    root.edgeStatusDescription = ko.computed(function () {
                        var desc = root.isProTier() ? root.proTypeName() : root.tierName(); // Tier
                        if (constants.IsGracePeriod) { // Grace period
                            desc = desc + ' (' + (root.DurationDays()) + ' days grace remaining)';
                        }
                        if (constants.IsComposed) { // Is Composed
                            desc = desc + ' (' + (root.DurationDays()) + ' days grace remaining. Minimum Tier: ' + constants.TierEnum[root.MinTierIndex()] + ')';
                        }
                        return desc;
                    }, this);

                    // Prefered Cashout Day
                    root.preferedCashoutDayName = ko.computed(function () {
                        return constants.PreferedCashoutDayNames[root.PreferedCashoutDay()];
                    });

                    // Leaderboard opt-in
                    root.leaderboardOptInName = ko.computed(function () {
                        return constants.LeaderboardOptInNames[root.LeaderboardOptIn()];
                    });

                    // Auto-convert FTPs
                    root.autoConvertName = ko.computed(function () {
                        return constants.AutoConvertNames[root.AutoConvert()];
                    });

                    // Note
                    root.AdminNote = ko.observable().extend({ required: true });

                    // Progress/loading anumation
                    root.progress = ko.observable(true);

                    // Dirty fields and validation
                    root.dirtyFlag = new ko.DirtyFlag([root.AdminNote]);
                    root.hasChanged = ko.computed(function () {
                        return root.dirtyFlag().isDirty();
                    });
                    root.isValid = ko.validatedObservable(root).isValid;

                    // Is editing?
                    root.isEditing = ko.observable(false);

                    var originalValues;

                    // Start Editing
                    root.startEdit = function () {
                        root.isEditing(true);
                        originalValues = ko.viewmodel.toModel(root);
                    };

                    // Cancel Editing
                    root.cancelEdit = ko.asyncCommand({
                        execute: function (complete) {
                            ko.viewmodel.updateFromModel(root, originalValues);
                            root.dirtyFlag().reset();
                            root.isEditing(false);
                            complete();
                        },

                        canExecute: function (isExecuting) {
                            return !isExecuting;
                        }
                    })

                    // Save
                    root.save = ko.asyncCommand({
                        execute: function (complete) {
                            root.dirtyFlag().reset();
                            complete();
                        },

                        canExecute: function (isExecuting) {
                            return !isExecuting && root.isValid() && root.hasChanged();
                        }
                    })

                },
                unmap: function (root) {
                    delete root.isProTier;
                    delete root.constants;
                    delete root.isValid;
                    delete root.cancelEdit;
                    delete root.save;
                    delete root.isEditing;
                    delete root.progress;
                    return root;
                }
            }
        }
    });

    return viewModel;
});


$(document).ready(function () {
    
    //var $element = $("#VIPInfoDiv");
    //vipInfoController($element, vipInfoModel);

});


//var vipInfoController = function ($element, viewModel) {

//    viewModel.DurationDays = viewModel.RemainingDays;
//    viewModel.errorMessage = null;

//    // Constants
//    var remainingDaysForBlackCard = 30;

//    // Elements
//    var $summaryPanel = $element.find("#VIPStatusDiv");
//    var $editPanel = $element.find("#VIPEditDiv");
//    var $progress = $element.find("#WIPProgressDiv");
//    var $errorSummary = $element.find("#ErrorSummary");

//    // Functions
//    var confirmStatusChange,
//        showSummaryPanel,
//        showEditPanel;

//    // Opens confirm status dialog and returns deferred object
//    confirmStatusChange = (function () {
//        var $dialog = $element.find("#WarningDiv");
//        $dialog.dialog({ height: "auto",
//            width: 350,
//            modal: true,
//            position: 'center',
//            autoOpen: false,
//            title: 'Confirm Change of Edge Status',
//            overlay: { opacity: 0.6, background: 'black' }
//            });
//        return function () {
//            var deferred = $.Deferred();
//            $dialog.dialog('option', 'buttons',
//                {
//                    "Confirm": function () {
//                        deferred.resolve();
//                        $(this).dialog("close");
//                    },
//                    "Cancel": function () {
//                        deferred.reject();
//                        $(this).dialog("close");
//                    }
//                });
//            $dialog.dialog("open");
//            return deferred.promise();
//        };
//    }());

//    // Shows the summary panel 
//    showSummaryPanel = (function () {

//        var $tier = $element.find("#VIPTierCell")
//                            .add($("#ctl00_MainContent_PlayerInformation_PlayerInformationSection_VIPStatusLabel"));
//        var $payday = $element.find("#EdgePaydayCell");
//        var $cashback = $element.find(".cashback-label");
//        var $leaderboardOptIn = $element.find("#LeaderboardOptInCell");
//        var $autoConvert = $element.find("#AutoConvertCell");
//        var $error = $element.find("#ErrorMessage");

//        $summaryPanel.click(function () {
//            if (viewModel.ReadOnly) { return; }
//            showEditPanel();
//        });

//        return function () {
//            // Is tier is a pro then will use ProType to get the description for the edge status
//            var tierDesc;
//            if (viewModel.Tier == viewModel.Tier_Pro) {
//                tierDesc = viewModel.ProTypeDescriptions[viewModel.ProType];
//            } else {
//                tierDesc = viewModel.TierDescriptions[viewModel.Tier];
//            }
//            // If there is a grace period then appends it 
//            var text = tierDesc;
//            if (viewModel.IsGracePeriod == true) {
//                text = tierDesc + ' (' + (viewModel.DurationDays) + ' days grace remaining)';
//            }
//            if (viewModel.IsComposed == true) {
//                text = tierDesc + ' (' + (viewModel.DurationDays) + ' days grace remaining. Minimum Tier: ' + viewModel.TierDescriptions[viewModel.MinTierIndex] + ')';
//            }
//            $tier.text(text);

//            $payday.text(viewModel.PreferedCashoutDaysNames[viewModel.PreferedCashoutDay]);

//            // If it is a Black Card Pro it must show the cashback
//            if (viewModel.Tier == viewModel.Tier_Pro) {
//                $cashback.text(viewModel.ProCashbackRate + "%");
//                $cashback.closest("tr").show();
//            } else {
//                $cashback.closest("tr").hide();
//            }
//            $leaderboardOptIn.text(viewModel.LeaderboardOptInNames[viewModel.CurrentLeaderboardOptInIndex]);
//            $autoConvert.text(viewModel.AutoConvertNames[viewModel.CurrentAutoConvertIndex]);
//            if (viewModel.errorMessage != null) {
//                $error.text(viewModel.errorMessage);
//                $error.show();
//            } else {
//                $error.hide();
//            }
//            $progress.hide();
//            $editPanel.hide();
//            $summaryPanel.show();
//        };
//    }());

//    // Show edit panel
//    showEditPanel = (function () {

//        var $tierSelect = $element.find("#TierSelect");
//        var $proTypeSelect = $element.find(".proType-select");
//        var $tierEditRaw = $element.find("#TierEditRaw");
//        var $cashback = $element.find(".cashback");
//        var $paydaySelect = $element.find("#DaySelect");
//        var $edgeFavourity = $element.find("#EdgeFavourity");
//        var $leaderboardOptInSelect = $element.find("#LeaderboardOptInSelect");
//        var $leaderboardOptInEdit = $element.find("#LeaderboardOptInEdit");
//        var $autoConvertSelect = $element.find("#AutoConvertSelect");
//        var $autoConvertEdit = $element.find("#AutoConvertEdit");
//        var $vipBreak = $element.find("#VIPBreakInput");
//        var $vipBreakEditRaw = $element.find("#VIPBreakEditRaw");
//        var $note = $element.find("#NoteTextArea");
//        var $save = $element.find("#SaveButton");
//        var $cancel = $element.find("#CancelButton");
//        var $error = $element.find(".msgerror");

//        // Functions
//        var submit,
//            showError;

//        // Build tier dorp down
//        $.each(viewModel.TierDescriptions, function (key, value) {
//            $tierSelect.append($("<option>").attr("value", key).text(value));
//        });
//        // Build pro type dorp down
//        $.each(viewModel.ProTypeDescriptions, function (key, value) {
//            $proTypeSelect.append($("<option>").attr("value", key).text(value));
//        });
//        $proTypeSelect.find("option:first").text("");
//        // Build payday dorp down
//        $.each(viewModel.PreferedCashoutDaysValues, function (i, value) {
//            $paydaySelect.append($("<option>").attr("value", value).text(viewModel.PreferedCashoutDaysNames[i]));
//        });
//        // Build leaderboard opt in drop down
//        $.each(viewModel.LeaderboardOptInValues, function (i, value) {
//            $leaderboardOptInSelect.append($("<option>").attr("value", value).text(viewModel.LeaderboardOptInNames[i]));
//        });
//        // Build auto convert drop down
//        $.each(viewModel.AutoConvertValues, function (i, value) {
//            $autoConvertSelect.append($("<option>").attr("value", value).text(viewModel.AutoConvertNames[i]));
//        });

//        // Tier changed event handler
//        $tierSelect.change(function () {
//            var selectedTier = $tierSelect.val();
//            if (selectedTier == viewModel.Tier_None) {
//                $vipBreak.val("0");
//                $vipBreak.hide();
//                $vipBreakEditRaw.hide();
//            } else {
//                var remainingDays = 0;
//                // We set a default grace period of 30 days when selecting Black Card.
//                if (selectedTier == viewModel.Tier_BlackCard) {
//                    remainingDays = remainingDaysForBlackCard;
//                }
//                $vipBreak.val(remainingDays);
//                $vipBreak.show();
//                $vipBreakEditRaw.show();
//            }
//            if (selectedTier == viewModel.Tier_Pro) {
//                $proTypeSelect.closest("tr").show();
//            } else {
//                $proTypeSelect.closest("tr").hide();
//            }
//            $proTypeSelect.trigger("change");
//        });

//        // Pro Type changed event handler
//        $proTypeSelect.change(function () {
//            var selectedTier = $tierSelect.val();
//            var selectedProType = $proTypeSelect.val();
//            if (selectedTier == viewModel.Tier_Pro && selectedProType != viewModel.ProType_Invalid) {
//                $cashback.closest("tr").show();
//                $cashback.val(viewModel.ProCashbackRate_Default);
//            } else {
//                $cashback.closest("tr").hide();
//            }
//            if (selectedTier == viewModel.Tier_Pro) {
//                if (selectedProType == viewModel.ProType_BlackCard) {
//                    $vipBreak.val(viewModel.GracePeriod_BlackCardPro);
//                } else {
//                    $vipBreak.val(viewModel.GracePeriod_Pros);
//                }
//            } else {
//                $vipBreak.val(0);
//            }
//        });

//        // Save
//        $save.click(function () {
//            var oldTier = viewModel.Tier;
//            var selectedTier = $tierSelect.val();
//            var isTimeable = selectedTier != viewModel.Tier_None && parseInt($vipBreak.val(), 10) < viewModel.DurationDays;

//            //This is when the tier is been decreased.
//            if (selectedTier < oldTier || isTimeable) {
//                confirmStatusChange().done(function () {
//                    submit();
//                });
//            } else {
//                submit();
//            }

//        });
//        // Cancel
//        $cancel.click(function () {
//            showSummaryPanel();
//        });
//        // Submit
//        submit = function () {
//            var selectedTier = $tierSelect.val();
//            var selectedProType = $proTypeSelect.val();
//            var vipBreakValue = $vipBreak.val();
//            var hasPayDayChanged = viewModel.PreferedCashoutDay != $paydaySelect[0].selectedIndex;
//            var hasStatusChanged = false;
//            var tierChanged = (selectedTier != viewModel.Tier);
//            var gracePeriodMissing = false;
//            var gracePeriodInvalid = false;

//            function isInRange(value, lowBoundaryInclusive, highBoundaryInclusive) {
//                var isInteger = new RegExp("^\\d+$").test(value);
//                if (!isInteger) { return false; }
//                return !(value < lowBoundaryInclusive || value > highBoundaryInclusive);
//            }
//            if (tierChanged || vipBreakValue != 0) {
//                hasStatusChanged = true;

//                if (selectedTier != viewModel.Tier_None) {
//                    gracePeriodMissing = (vipBreakValue.length == 0);
//                    gracePeriodInvalid = !isInRange(vipBreakValue, 1, viewModel.MaxGracePeriod);
//                } else {
//                    $vipBreak.val("0");
//                }
//            }
//            if (selectedTier == viewModel.Tier_Pro && selectedProType == viewModel.ProType_Invalid) {
//                showError("Please select a Pro Type.");
//                return;
//            }
//            if (selectedTier == viewModel.Tier_Pro && !isInRange($cashback.val(), 0, viewModel.ProCashbackRate_Max)) {
//                showError("Invalid Cashback");
//                return;
//            }
//            if (gracePeriodMissing || gracePeriodInvalid) {
//                showError("Invalid Grace period");
//                return;
//            }
//            if (!$note.val()) {
//                showError("Please enter a Note.");
//                return;
//            }
//            viewModel.JustUpdatePayDay = false;
//            if (hasPayDayChanged) {
//                /*jslint bitwise: true*/
//                viewModel.JustUpdatePayDay = hasPayDayChanged ^ hasStatusChanged;
//                /*jslint bitwise: false*/
//            }

//            // Populate view model
//            var oldVm = $.extend(true, {}, viewModel);
//            viewModel.Tier = parseInt(selectedTier, 10);
//            viewModel.TierChanged = tierChanged;
//            viewModel.ProType = parseInt(selectedProType, 10);
//            viewModel.ProTypeChanged = $proTypeSelect.val() != viewModel.ProType;
//            viewModel.ProCashbackRate = parseInt($cashback.val(), 10);
//            viewModel.DurationDays = parseInt(vipBreakValue, 10);
//            viewModel.AdminNote = $note.val();
//            viewModel.PreferedCashoutDay = $paydaySelect[0].selectedIndex;
//            viewModel.NewLeaderboardOptInIndex = $leaderboardOptInSelect[0].selectedIndex;
//            viewModel.NewAutoConvertIndex = $autoConvertSelect[0].selectedIndex;

//            $error.hide();
//            $progress.show();

//            var handleError = function (errorMessage) {
//                viewModel = $.extend(true, {}, oldVm);
//                viewModel.DurationDays = viewModel.RemainingDays;
//                showError(errorMessage);
//            };

//            /*jslint unparam: true*/
//            $.ftUtils.watCallbackAction(ApplicationPath, "PlayerDetailsCallback", "UpdateVIPInfo", JSON.stringify(viewModel))
//                .done(function (response) {
//                    if (response.ErrorMessage === undefined) {
//                        viewModel.RemainingDays = viewModel.DurationDays;
//                        viewModel.TierChanged = false;
//                        viewModel.ProTypeChanged = false;
//                        viewModel.CurrentLeaderboardOptInIndex = viewModel.NewLeaderboardOptInIndex;
//                        viewModel.CurrentAutoConvertIndex = viewModel.NewAutoConvertIndex;
//                        $error.hide();
//                        showSummaryPanel();
//                    } else {
//                        handleError(response.ErrorMessage);
//                    }
//                }).fail(function (request, errorType, errorThrown) {
//                    handleError(errorThrown);
//                }).always(function (request, errorType, errorThrown) {
//                    $progress.hide();
//                });
//            /*jslint unparam: false*/
//        };

//        // Show error
//        showError = function (message) {
//            $error.text(message);
//            $error.show();
//        };

//        return function () {
//            $tierSelect.val(viewModel.Tier);
//            $proTypeSelect.val(viewModel.ProType);
//            $tierEditRaw.show();
//            $paydaySelect.val(viewModel.PreferedCashoutDaysValues[viewModel.PreferedCashoutDay]);
//            $edgeFavourity.show();
//            $leaderboardOptInSelect.val(viewModel.LeaderboardOptInValues[viewModel.CurrentLeaderboardOptInIndex]);
//            $leaderboardOptInEdit.show();
//            $autoConvertSelect.val(viewModel.AutoConvertValues[viewModel.CurrentAutoConvertIndex]);
//            $autoConvertEdit.show();
//            $error.hide();

//            // Normal VIP
//            if (viewModel.Tier == viewModel.Tier_None) {
//                $vipBreak.val(viewModel.MaxGracePeriod);
//                var selectedTier = $tierSelect.val();
//                if (selectedTier == viewModel.Tier_None) {
//                    $vipBreak.val("0");
//                    $vipBreak.hide();
//                    $vipBreakEditRaw.hide();
//                }
//            } else {
//                $vipBreak.val(viewModel.RemainingDays);
//            }

//            // Clear note
//            $note.val("");
//            $tierSelect.trigger("change");
//            if (viewModel.ProCashbackRate) {
//                $cashback.val(viewModel.ProCashbackRate);
//            }
//            if (viewModel.DurationDays) {
//                $vipBreak.val(viewModel.DurationDays);
//            }
//            $errorSummary.hide();
//            $progress.hide();
//            $summaryPanel.hide();
//            $editPanel.show();
//        };
//    }());

//    showSummaryPanel();
//};


//$(document).ready(function () {
//    vipInfoController($("#VIPInfoDiv"), vipInfoModel);
//});

// To be refatored into an utilities file

