const playerRow = Vue.component('player-row', {
    props: ['data'],
    template: 
            `<tr readonly :id="'playerMainTableRow_' + data.Id" v-bind:data-playerid="data.Id">
                    <td> {{data.Name}}</td > 
                    <td>{{ data.Email }}</td> 
                    <td>{{data.Group}}</td> 
                    <td>&nbsp;</td>
            </tr > `,
    replace: true
});