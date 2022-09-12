<template>
    <v-container>
        <v-row flex align-center>
            <v-col xs12 sm4 elevation-6>
                <v-card style="height:75vh;">
                    <v-card-title class="justify-center display-1 matchingText--text">Register</v-card-title>
                    <v-card-text class="pt-4">
                        <div>
                            <v-form v-model="valid" ref="form">
                                <v-text-field color="primary" label="Enter your first name" v-model="firstname"
                                    :rules="[rules.required]" required></v-text-field>
                                <v-text-field color="primary" label="Enter your surname" v-model="lastname"
                                    :rules="[rules.required]" required></v-text-field>
                                <v-text-field color="primary" label="Enter your e-mail address" v-model="email"
                                    :rules="[rules.required]" required></v-text-field>
                                <v-text-field color="primary" label="Enter your password" v-model="password" min="4"
                                    :append-icon="show1 ? 'mdi-eye' : 'mdi-eye-off'"
                                    :rules="[rules.required, rules.min]" :type="show1 ? 'text' : 'password'" required
                                    @click:append="show1 = !show1"></v-text-field>
                                <v-row justify="center">
                                    <v-btn @click="register"
                                        :class=" { 'matchingText white--text': valid, disabled: !valid }"
                                        :disabled="valid===false">Register</v-btn>
                                </v-row>
                            </v-form>
                        </div>
                        <div class="text-center" style="margin-top:5vh;">{{status}}</div>
                    </v-card-text>
                </v-card>
            </v-col>
        </v-row>
    </v-container>
</template>
<script>
    import fetcher from "../mixins/fetcher";
    export default {
        name: "Register",
        data() {
            return {
                email: "",
                password: "",
                firstname: "",
                lastname: "",
                show1: false,
                rules: {
                    required: value => !!value || "Required.",
                    min: v => v.length >= 4 || "Min 4 characters"
                },
                valid: false,
                status: ""
            };
        },
        methods: {
            register: async function () {
                try {
                    this.status = "registering with server";
                    let CustomerHelper = {
                        firstname: this.firstname,
                        lastname: this.lastname,
                        email: this.email,
                        password: this.password
                    };
                    let payload = await this.$_postdata("register", CustomerHelper); // in mixin
                    this.status = payload.token;
                } catch (err) {
                    console.log("here" + err);
                    this.status = `Error has occured: ${err}`;
                }
            }
        },
        mixins: [fetcher]
    };
</script>