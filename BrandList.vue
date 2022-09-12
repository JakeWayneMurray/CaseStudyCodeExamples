<template>
    <v-container fluid>
        <v-row justify="center" class="text-center display-2" style="margin-top:.5em;">Brands</v-row>
        <v-row justify="center">
            <v-col class="title text-center matchingText--text" >{{status}}</v-col>
        </v-row>
        <v-row justify="center">
            <v-col class="text-left display-1">
                <v-select :items="brands" item-text="name" style="max-height: 50%;" item-value="id" @input="changeBrand"
                    v-model="selectedid"></v-select>
            </v-col>
        </v-row>
        <div v-if="products.length > 0">
            <v-row justify="center" class="text-center headline">Products</v-row>
            <v-row justify="center" style="margin-top:1vh;">
                <v-col class="text-left display-2">
                    <v-list style="max-height: 60vh;" class="overflow-y-auto">
                        <v-list-item-group>
                            <v-list-item @click="popDialog(item)" v-for="(item, i) in products" :key="i">
                                <v-card width="45vh" class="mx-auto" height="15vh"
                                    style="margin-top:1vh;">
                                    <v-row>
                                        <v-card-title style="float; padding-right:2vh" class="subtitle" v-text="item.productName">
                                        </v-card-title>
                                        <v-img v-if="item.graphicName" :src="require(`../assets/${item.graphicName}`)"
                                            style="margin-right:2vh;margin-top:1vh;margin-bottom:1vh"
                                            height="13vh" width="12vh">
                                        </v-img>
                                    </v-row>
                                </v-card>
                            </v-list-item>
                        </v-list-item-group>
                    </v-list>
                </v-col>
            </v-row>
        </div>
        <v-dialog v-model="dialog" v-if="selectedItem" justify="center" align="center">
            <v-card>
                <v-row>
                    <v-spacer></v-spacer>
                    <v-btn text @click="dialog = false" style="font-size:XX-large;margin:1vw;">X</v-btn>
                </v-row>
                <v-row>
                    <!--<v-img :src="require('../assets/burger.jpg')" height="15vh" width="15vh" contain aspect-ratio="1" />-->
                </v-row>
                <v-row justify="center" class="title">{{selectedItem.productName}}</v-row>
                <div style="margin:3vw;border:ridge;padding-left:5vw;padding-right:5vw;">
                    <v-row justify="center" style="padding-top:2vh;">
                        <v-img max-width="25vw" v-if="selectedItem.graphicName"
                            :src="require(`../assets/${selectedItem.graphicName}`)">
                        </v-img>
                    </v-row>
                    <v-row>
                        <v-col />
                        <v-col />
                        <v-col>MSRP:</v-col>
                        <v-col>{{selectedItem.msrp | currency}}</v-col>
                    </v-row>
                    <v-divider />
                    <v-row>
                        <v-col />
                        <v-col />
                        <v-col>Qty:</v-col>
                        <v-col>{{selectedItem.qtyOnHand}}</v-col>
                    </v-row>
                    <v-row style="margin:3vw;padding-left:5vw;padding-right:5vw;" />

                    Description:
                    <v-row>
                        <v-col>{{selectedItem.description}}</v-col>
                    </v-row>
                </div>
                <v-row style="margin-left:3vw;">
                    <v-col>Qty:</v-col>
                    <v-col>
                        <input type="number" maxlength="3" placeholder="enter qty" size="3"
                            style="width: 15vw;border-bottom:solid;text-align:right" v-model="qty" />
                    </v-col>
                </v-row>
                <v-row justify="center" align="center" style="margin-bottom:2vh;margin-left:3vw;">
                    <v-col>
                        <v-btn justify="left" medium outlined color="default" @click="addToCart">Add To Cart</v-btn>
                    </v-col>
                    <v-col>
                        <v-btn medium outlined color="default" @click="viewCart">View Cart</v-btn>
                    </v-col>

                </v-row>

                <v-col cols="7"></v-col>

                <v-row justify="center" align="center" style="padding-bottom:5vh;">{{this.dialogStatus}}</v-row>
            </v-card>
        </v-dialog>

        <v-footer absolute class="headline">
            <v-col class="text-center" cols="12">
                &copy;{{ new Date().getFullYear() }} —
                INFO3067
            </v-col>

        </v-footer>
    </v-container>
</template>
<script>
    import fetcher from "../mixins/fetcher";
    export default {
        name: "BrandList",
        data() {
            return {
                brands: [],
                status: {},
                selectedid: 0,
                products: [],
                dialog: false,
                selectedItem: {},
                dialogStatus: "",
                qty: 0,
                cart: []
            };
        },
        mixins: [fetcher],
        mounted: async function () {
            try {
                this.status = "fetching categories from server...";
                this.brands = await this.$_getdata("brand"); // getdata is in mixin
                this.status = `loaded ${this.brands.length + 1} brands`;
                this.brands.unshift({
                    name: "Current Categories",
                    id: 0
                });
            } catch (err) {
                console.log(err);
                this.status = `Error has occured: ${err.message}`;
            }
            if (sessionStorage.getItem("cart")) {
                this.tray = await JSON.parse(sessionStorage.getItem("cart"));
            }
        },
        methods: {
            addToCart: function () {
                const index = this.cart.findIndex(
                    // is item already on the cart
                    item => item.id === this.selectedItem.id
                );
                if (this.qty !== "0") {
                    index === -1 ?
                        this.cart.push({
                            id: this.selectedItem.id,
                            name: this.selectedItem.name,
                            qty: parseInt(this.qty),
                            item: this.selectedItem
                        }) // add
                        :
                        (this.cart[index] = {
                            // replace
                            id: this.selectedItem.id,
                            name: this.selectedItem.name,
                            qty: parseInt(this.qty),
                            item: this.selectedItem
                        });
                    this.dialogStatus = `${this.qty} item(s) added`;
                } else {
                    index === -1 ? null : this.cart.splice(index, 1); // remove
                    this.dialogStatus = `item(s) removed`;
                }
                sessionStorage.setItem("cart", JSON.stringify(this.cart));
            },
            viewCart: function () {
                this.$router.push({
                    name: "cart"
                })
            },
            popDialog: function (item) {
                this.dialogStatus = "";
                this.dialog = !this.dialog;
                this.selectedItem = item;
            },
            changeBrand: async function (brandid) {
                if (brandid > 0) {
                    // don't use arrow function here
                    try {
                        this.status = `fetching items for ${brandid}...`;
                        this.products = await this.$_getdata(`product/${brandid}`);
                        this.status = `found ${this.products.length} items`;
                    } catch (err) {
                        console.log(err);
                        this.status = `Error has occured: ${err.message}`;
                    }
                }
            }
        }

    };
</script>