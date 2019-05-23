const playerRow = Vue.component('player-row', {
    props: ['data', 'canEdit'],
    template: 
            `<tr readonly :id="'playerMainTableRow_' + data.Id" v-bind:data-playerid="data.Id">
                    <td class="align-middle"><input :readonly="!canEdit" :id="'playerMainTableRowName_' + data.Id" type="text" class="required col form-control form-control-sm" v-model:value="data.Name" placeholder="Name" data-valuetype="Name" data-originalvalue="data.Name" @dblclick="unlockInpt(canEdit)"/></td>
                    <td class="align-middle"><input readonly :id="'playerMainTableRowEmail_' + data.Id" type="text" class="col form-control form-control-sm" :value="data.Email" placeholder="Email Address" data-valuetype="Email" data-originalvalue="data.Email" ondblclick="UnlockInput(this);" ontouchstart="UnlockInput(this);" /></td>
                    <td class="align-middle"><input readonly :id="'playerMainTableRowGroup_' + data.Id" type="text" class="col form-control form-control-sm" :value="data.Group" placeholder="Group" data-valuetype="Group" data-originalvalue="data.Group" ondblclick="UnlockInput(this);" ontouchstart="UnlockInput(this);" /></td>
                    <td class="d-flex justify-content-center align-content-center">
                        <a href="#" title="Delete" style="font-weight:bold" class="btn btn-danger fas fa-trash-alt m-0 mr-1 p-2" onclick="RemovePrompt(this);"></a>
                        <a v-if="data.Id > 0" href="#" title="Undo" style="font-weight:bold" class="btn btn-warning fas fa-undo m-0 ml-1 p-2" :onclick="'UndoPrompt(' + data.Id + ');'"></a>
                    </td>
            </tr > `,
    methods: {
        unlockInpt: function (canEdit) {
            debugger
            canEdit = true;
            
        },
    },
    watch: {
        canEdit: function (old, newthing) {
            debugger
        }
    }
});


//ontouchstart = "UnlockInput(this);" onfocus = "$(this).popover('dispose');" 
