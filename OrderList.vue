<template>
    <v-container fluid>
        <v-row justify="center" class="text-center headline matchingText--text" style="margin-top:2vh;">Saved Orders
        </v-row>
        <v-row justify="center">
            <v-col class="title text-center" style="color:matchingText">{{status}}</v-col>
        </v-row>
        <div v-if="orders.length > 0">
            <v-row justify="center" style="margin:3vw;">
                <v-col class="text-center headline" cols="3">Id</v-col>
                <v-col cols="5" class="text-center headline">Date</v-col>
                <v-col cols="4" class="text-left headline">Amount</v-col>
            </v-row>
            
                    <v-divider style="margin-bottom:3vh"/>
            <v-row>
                <v-col>
                    <v-list style="max-height: 50vh;margin-top:-3vh;" class="overflow-y-auto">
                        <v-list-item-group>
                            <v-list-item v-for="(order, i) in orders" :key="i" @click="popDialog(order)">
                                <v-card width="43vh" height="6vh" style="margin-top:1vh; padding-left:2vh; padding-right:2vh">
                                    <v-row >
                                        <v-col cols="3" v-text="order.id">
                                        </v-col>
                                        <v-col cols="5" v-text="formatDate(order.orderDate)" >
                                        </v-col>
                                        <v-col cols="4" class="text-right">{{order.orderAmount | currency}}
                                        </v-col>
                                    </v-row>
                                </v-card>
                            </v-list-item>
                        </v-list-item-group>
                    </v-list>
                </v-col>
            </v-row>
            <v-dialog v-model="dialog" v-if="selectedOrder" justify="center" align="center" width="70vh">
                <v-card>
                    <v-row>
                        <v-spacer></v-spacer>
                        <v-btn text @click="dialog = false" style="fmargin:2vw;">X</v-btn>
                    </v-row>
                    <v-row align="center"
                        class="matchingText--text" style="padding-left:3vh">Order: #{{selectedOrder.id}}
                    </v-row>
                     <v-row align="center"
                        class="matchingText--text" style="padding-left:3vh" v-text="formatDate(selectedOrder.orderDate)">
                    </v-row>
                    <v-row>
                        <v-col justify="center" align="center">
                            <v-img :src="require('../assets/cart.png')" style="height:10vh;width:10vh;padding:0;"
                                aspect-ratio="1" />
                        </v-col>
                    </v-row>
                    <div style="margin:2vw;">
                        <v-row style="margin-top:2vh;background-color:lightBlue;">
                            <v-col cols="3" class="text-center font-weight-bold">Name</v-col>
                            <v-col cols="3" class="text-center font-weight-bold">MSRP</v-col>
                            <v-col cols="1" class="text-center font-weight-bold">S</v-col>
                            <v-col cols="1" class="text-center font-weight-bold">O</v-col>
                            <v-col cols="1" class="text-center font-weight-bold">B</v-col>
                            <v-col cols="3" class="text-center font-weight-bold">Extended</v-col>


                        </v-row>
                        <v-row v-for="(detail, i) in details" :key="i"
                            style="margin-bottom:0">
                            <v-col cols="3" class="text-center caption">{{detail.productName}}</v-col>
                            <v-col cols="3" class="text-center caption">{{detail.productPrice}}</v-col>
                            <v-col cols="1" class="text-center caption">{{detail.qtySold}}</v-col>
                            <v-col cols="1" class="text-center caption">{{detail.qtyOrdered}}</v-col>
                            <v-col cols="1" class="text-center caption">{{detail.qtyBackOrdered}}</v-col>
                            <v-col cols="3" class="text-center caption">{{detail.qtySold * detail.productPrice | currency}}</v-col>
                        </v-row>
                    </div>
                    <v-divider />
                    <div style="padding-top:4vw;margin:3vw;padding-left:5vw;padding-right:5vw">
                        <v-row style="margin-bottom:0;margin-top:-2vh;">
                            <v-col cols="8" class="font-weight-bold text-right">Price:</v-col>
                            <v-col cols="4" class="text-right">{{selectedOrder.orderAmount | currency}}</v-col>
                        </v-row>
                        <v-row style="margin-bottom:0;margin-top:-2vh;">
                            <v-col cols="8" class="font-weight-bold text-right">Tax:</v-col>
                            <v-col cols="4" class="text-right">{{selectedOrder.orderAmount * 0.13 | currency}}</v-col>
                        </v-row>
                        <v-row style="margin-bottom:0;margin-top:-2vh;">
                            <v-col cols="8" class="font-weight-bold text-right">Order Total:</v-col>
                            <v-col cols="4" class="text-right">{{selectedOrder.orderAmount * 1.13 | currency}}</v-col>
                        </v-row>
                    </div>
                    <v-row justify="center" align="center" style="padding-bottom:2vh;">{{this.dialogStatus}}</v-row>
                </v-card>
            </v-dialog>
        </div>
    </v-container>
</template>
<script>
    import fetcher from "../mixins/fetcher";
    import datertn from "../mixins/datertn";
    export default {
        name: "OrderList",
        data() {
            return {
                orders: [],
                status: {},
                details: [],
                selectedOrder: {},
                dialog: false,
                dialogStatus: {}
            };
        },
        mixins: [fetcher, datertn],
        beforeMount: async function () {
            try {
                let customer = JSON.parse(sessionStorage.getItem("customer"));
                this.status = "fetching orders from server...";
                let route = this.$_buildRouteWithParams("order", customer.email.trimEnd());
                this.orders = await this.$_getdata(route.slice(0, -1)); // in mixin
                this.status = `loaded ${this.orders.length + 1} order`;
            } catch (err) {
                console.log(err);
                this.status = `Error has occured: ${err.message}`;
            }
        },
        
    methods: {
        selectOrder: async function (orderid) {
            if (orderid > 0) {
                // don't use arrow function here
                try {
                    let customer = JSON.parse(sessionStorage.getItem("customer"));
                    this.status = `fetching details for customer ${orderid}...`;
                    let route = this.$_buildRouteWithParams(
                        "order",
                        orderid,
                        customer.email.trimEnd()
                    );
                    this.details = await this.$_getdata(route.slice(0, -1)); // remove end /
                    this.status = `found order ${orderid} details`;
                } catch (err) {
                    console.log(err);
                    this.status = `Error has occured: ${err.message}`;
                }
            }
        },
        popDialog: async function (order) {
            this.dialogStatus = "";
            this.dialog = !this.dialog;
            this.selectedOrder = order;
            await this.selectOrder(order.id);
        }
    }
    };
</script>