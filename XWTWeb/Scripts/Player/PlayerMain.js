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
}


//Add new player row
var intNewPlayerCount = 0;
function AddNewPlayer() {
    intNewPlayerCount++;

    //intNewPlayerCount++;
    var player = {
        Id: intNewPlayerCount * -1,
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

function UpdatePlayer(btn) {
    btn = btn || "";

    if (CheckRequiredFields()) {

        var data = JSON.stringify(app.items);

        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/Player/UpdatePlayerData');
        xhr.setRequestHeader('Content-Type', 'application/json');
        xhr.onload = function () {
            if (xhr.readyState == 4) {
                SetIsDirty(false);
                if (btn != "") {
                    $(btn).click();
                    location.href = $(btn).attr('href');
                }
                else location.reload();
            }
        };
        xhr.send(data);
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
