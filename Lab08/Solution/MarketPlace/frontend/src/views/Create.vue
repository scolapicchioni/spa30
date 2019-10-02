<template>
<v-form>
  <v-container>
    <v-row>
      <v-col>
        <v-text-field label="Product Name" v-model="product.name"></v-text-field>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-textarea label="Description" v-model="product.description" hint="Product Description"></v-textarea>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-text-field label="Product Price" v-model.number="product.price"></v-text-field>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-file-input accept="image/*" label="Select Picture from device" @change="onFileChanged"></v-file-input>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-btn text icon @click="capture">
          <v-icon>mdi-camera</v-icon>
        </v-btn>
        <video ref="videoElement" width="100%"></video>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-img :src="product.picture"></v-img>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-btn @click="insertProduct" color="primary">INSERT PRODUCT</v-btn>
      </v-col>
    </v-row>
  </v-container>
</v-form>
</template>

<script>
import datalayer from '@/datalayer'
export default {
  data () {
    return {
      product: {
        id: 0,
        name: '',
        description: '',
        picture: '',
        price: 0
      },
      selectedFile: null
    }
  },
   async mounted () {
    const mediaStream = await navigator.mediaDevices.getUserMedia({ video: true })
    this.mediaStream = mediaStream
    this.$refs.videoElement.srcObject = mediaStream
    this.$refs.videoElement.play()
  },
  destroyed () {
    const tracks = this.mediaStream.getTracks()
    tracks.map(track => track.stop())
  },
  methods: {
    async insertProduct () {
      await datalayer.insertProduct(this.product)
      this.$router.push('/')
    },
    onFileChanged (event) {
      console.log(event)
      this.selectedFile = event
      this.updateImage()
    },
    updateImage () {
      const reader = new FileReader()
      reader.onload = () =>  this.product.picture = reader.result 
      reader.readAsDataURL(this.selectedFile)
    },
    async capture () {
      const mediaStreamTrack = this.mediaStream.getVideoTracks()[0]
      const imageCapture = new window.ImageCapture(mediaStreamTrack)
      this.selectedFile = await imageCapture.takePhoto()
      this.updateImage()
    }
  }
}
</script>

<style>

</style>