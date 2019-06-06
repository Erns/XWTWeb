const app = new Vue({
    el: '#mainDiv',
    data: {
        items: [{ Id: 1, Name: 'Test 1' }, { Id: 2, Name: 'Test 2' }]
    },
    mounted: function () {
        var xhr = new XMLHttpRequest();
        xhr.open('GET', '/Player/GetPlayerData');
        xhr.setRequestHeader('Content-Type', 'application/json');
        xhr.onload = function () {
            if (xhr.readyState == 4) {
                callback(xhr.responseText);
            }
        };
        xhr.send();
    }
});

function callback(responseText) {
    app.items = JSON.parse(responseText);
    debugger
}


//Add new player row
var intNewPlayerCount = 0;
function AddNewPlayer() {
    //var rowTemplate = $("#playerMainTableRowTemplate").clone();

    //rowTemplate.attr('id', 'newPlayerRow_' + intNewPlayerCount)
    //    .prependTo($("#playerMainTable tbody"))
    //    .show();

    //intNewPlayerCount++;
    var player = {
        Id: 0,
        Name: '',
        Email: '',
        Group: '',
        Active: true,
        DateDeleted: null,
        CanEdit: true
    };

    app.items.unshift(player);
    SetIsDirty(true);
}

//Delete player
function RemovePrompt(btn) {
    bootbox.confirm({
        size: "small",
        message: "Are you sure you want to remove this player?",
        callback: function (result) {
            if (result) {
                RemovePlayerRow(btn);
            }
        }
    });
}

//Delete player Row
function RemovePlayerRow(btn) {
    $(btn).parent().parent().hide();
    $('.popover').remove();
    SetIsDirty(true);
}

//Undo changes made
function UndoPrompt(Id) {
    bootbox.confirm({
        size: "small",
        message: "Do you want to undo changes to this player?",
        callback: function (result) {
            if (result) {
                $("#playerMainTableRow_" + Id + " td input").each(function () {
                    $(this).val($(this).data('originalvalue'));
                    $(this).prop('readonly', true);
                    $(this).parent().parent().prop('readonly', true).attr('readonly', true);
                });
            }
        }
    });
}

//Unlock field to edit
function UnlockInput(inpt) {
    if ($(inpt).prop('readonly')) {
        $(inpt).prop('readonly', false);
        $(inpt).parent().parent().prop('readonly', false).attr('readonly', false);
        $(inpt).blur().focus();
        var tmp = $(inpt).val();
        $(inpt).val('').val(tmp);
        SetIsDirty(true);
    }
}

        //function UpdatePlayer(btn) {
        //    btn = btn || "";

        //    var jsonData = [];
        //    var playerId;

        //    //Pull info from edited Players
        //    $("tr[id^=playerMainTableRow_]").each(function () {
        //        if ((!$(this).attr('readonly') && !$(this).prop('readonly')) || !$(this).is(':visible')) {
        //            playerId = $(this).data('playerid');
        //            var player = {
        //                Id: playerId,
        //                Name: $("#playerMainTableRowName_" + playerId).val(),
        //                Email: $("#playerMainTableRowEmail_" + playerId).val(),
        //                Group: $("#playerMainTableRowGroup_" + playerId).val(),
        //                Active: true,
        //                DateDeleted: null
        //            };

        //            //If not visible, then deleted
        //            if (!$(this).is(':visible')) {
        //                player.Active = false;
        //                player.DateDeleted = new Date().toLocaleString();
        //            }

        //            jsonData.push(player);
        //        }
        //    });

        //    //Pull info for new Players
        //    $("tr[id^=newPlayerRow_]").each(function () {
        //        if ($(this).is(':visible')) {
        //            var player = {
        //                Id: 0,
        //                Name: $(this).find("input[data-valuetype='Name']").val(),
        //                Email: $(this).find("input[data-valuetype='Email']").val(),
        //                Group: $(this).find("input[data-valuetype='Group']").val(),
        //                Active: true
        //            };
        //            jsonData.push(player);
        //        }
        //    });


        //    if (CheckRequiredFields()) {
        //        BBoxLoading();

        //        //Update player info
        //        $.ajax({
        //            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        //            dataType: "json",
        //            type: "POST",
        //            url: "/Player/UpdatePlayerData",
        //            data: { players: JSON.stringify(jsonData) },
        //            complete: function (data) {
        //                SetIsDirty(false);
        //                if (btn != "") {
        //                    $(btn).click();
        //                    location.href = $(btn).attr('href');
        //                }
        //                else location.reload();
        //            }
        //        });
        //    }


        //}
