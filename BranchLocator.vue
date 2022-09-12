<template>
  <v-container>
    <v-row align="center" justify="center">
      <v-col class="title text-center matchingText--text">Find Branches Near You</v-col>
    </v-row>
      <v-row>
      <v-col justify="center" align="center">
        <v-img
          :src="require('../assets/house.png')"
          style="height:12vh;width:12vh;"
          aspect-ratio="1"
        />
      </v-col>
    </v-row>
    <v-row justify="center">
      <v-col>
        <v-text-field color="primary" label="Enter address" v-model="address" required></v-text-field>
      </v-col>
    </v-row>
    <v-row justify="center" style="font-size:small;color:gray;">Example: London,On OR N5X-2C5</v-row>
    <v-row justify="center">
      <v-btn
        style="margin-top:4vh"
        @click="popDialog()"
        :class=" { 'matchingText white--text' :valid, disabled: !valid }"
      >Locate</v-btn>
    </v-row>
    <v-dialog v-model="dialog" justify="center" align="center">
        <v-card>
          <v-row>
            <v-spacer></v-spacer>
            <v-btn text @click="dialog = false" style="font-size:XX-large;margin:1vw;">X</v-btn>
          </v-row>
           <v-row
      class="display-1"
      align="center"
      justify="center"
      style="margin-top:2vh;color:gray"
    >{{status}}</v-row>
    <v-row justify="center">
      <div
        id="map"
        ref="map"
        class="googlemap"
        style="margin-top:2vh;margin-bottom:5vh;height:250px;width:300px;min-height:300px;"
      ></div>
    </v-row>
        </v-card>
    </v-dialog>
  </v-container>
</template>
<script>
import fetcher from "../mixins/fetcher.js";
export default {
  data() {
    return {
      details:[],
      status: "",
      i2:0,
      address: "N5X-2C5",
      valid: true,
      lat: 0.0,
      lng: 0.0,
      dialog: false,
      dialogStatus: {}
    };
  },
  mixins:[fetcher],
  methods: {
    LocateBranches() {
      try {
        let geocoder = new window.google.maps.Geocoder();
      geocoder.geocode({ address: this.address }, async (results, status) => {
        if (status === window.google.maps.GeocoderStatus.OK) {
        this.lng = results[0].geometry.location.lng();
        this.lat = results[0].geometry.location.lat();
          

         this.status = "fetching branches from server...";
         let route = this.$_buildRouteWithParams("branch" + "/" + this.lat + "/" + this.lng);
         console.log(this.details.length);
         this.details = await this.$_getdata(route.slice(0, -1));
         console.log(this.details.length);
          let myLatLng = new window.google.maps.LatLng(this.lat, this.lng);
          let options = {
            zoom: 10,
            center: myLatLng,
            mapTypeId: window.google.maps.MapTypeId.ROADMAP
          };
         this.status = `loaded ${this.details.length} branches`;


          this.map = new window.google.maps.Map(this.$refs["map"], options);
          let center = this.map.getCenter();
          this.map.setCenter(center);
          this.details.map(detail => {
            this.i2++;
            let img = `/img/marker${this.i2}.png`;
            let marker = new window.google.maps.Marker({
              position: new window.google.maps.LatLng(
                detail.longitude,
                detail.latitude
              ),
            
              animation: window.google.maps.Animation.DROP,
              title: `branch#${detail.id} ${detail.street}, ${detail.city},${detail.region}`,
              icon: img,
              html: `<div>branch#${detail.id}<br/>${detail.street}, ${
                detail.city
              } ${detail.distance.toFixed(2)} km</div>`,
              visible: true
              
            });
            
            let infowindow = new window.google.maps.InfoWindow({ content: "" });
            window.google.maps.event.addListener(marker, "click", () => {
              infowindow.setContent(marker.html);
              infowindow.close();
              infowindow.open(this.map, marker);
            });

            marker.setMap(this.map);

            let center = this.map.getCenter();
            this.map.setCenter(center);
            window.google.maps.event.trigger(this.map, "resize");
          });
        }
      });
    }
       catch (err) {
        console.log(err);
      }
    },
    popDialog() 
{ 
  this.dialogStatus = "";
  this.dialog = !this.dialog;
  this.LocateBranches();
  this.i2 = 0;
}
  }
};
</script>