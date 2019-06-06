const playerRow = Vue.component('player-row', {
    props: ['data'],
    template: 
            `<tr :id="'playerMainTableRow_' + data.Id" v-bind:data-playerid="data.Id">
                    <td class="align-middle"><input v-bind:readonly="!data.CanEdit" :id="'playerMainTableRowName_' + data.Id" type="text" class="required col form-control form-control-sm" v-model:value="data.Name" :data-original="data.OriginalName" placeholder="Name" data-valuetype="Name" v-on:dblclick="unlockInpt(data)"/></td>
                    <td class="align-middle"><input v-bind:readonly="!data.CanEdit" :id="'playerMainTableRowEmail_' + data.Id" type="text" class="col form-control form-control-sm" v-model:value="data.Email" :data-original="data.OriginalEmail" placeholder="Email Address" data-valuetype="Email" data-originalvalue="data.Email" v-on:dblclick="unlockInpt(data)" /></td>
                    <td class="align-middle"><input v-bind:readonly="!data.CanEdit" :id="'playerMainTableRowGroup_' + data.Id" type="text" class="col form-control form-control-sm" v-model:value="data.Group" :data-original="data.OriginalGroup" placeholder="Group" data-valuetype="Group" data-originalvalue="data.Group" v-on:dblclick="unlockInpt(data)" /></td>
                    <td class="d-flex justify-content-center align-content-center">
                        <a href="#" title="Delete" style="font-weight:bold" class="btn btn-danger fas fa-trash-alt m-0 mr-1 p-2" onclick="RemovePrompt(this);"></a>
                        <a v-if="data.Id > 0" href="#" title="Undo" style="font-weight:bold" class="btn btn-warning fas fa-undo m-0 ml-1 p-2" v-on:click="undoPrompt(data)"></a>
                        <b-button v-b-modal.modal-1>testing</b-button>
                    </td>
            </tr > `,
    methods: {
        unlockInpt: function (data) {
            data.CanEdit = true;
            SetIsDirty(true);
        },
        undoPrompt: function (data) {
            data.CanEdit = false;
            data.Name = data.OriginalName;
            data.Email = data.OriginalEmail;
            data.Group = data.OriginalGroup;
        }
    }
});
