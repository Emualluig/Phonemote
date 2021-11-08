import * as React from 'react';
import { StyleSheet, Text, View, Button, Dimensions, TouchableOpacity, TouchableOpacityBase, TouchableWithoutFeedback } from 'react-native';
import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { AntDesign } from '@expo/vector-icons'; // More about icons: https://docs.expo.dev/guides/icons/
import { BarCodeScanner } from 'expo-barcode-scanner';

const Stack = createNativeStackNavigator();

const validateQrCode = (type: string, data: string) : boolean => {
  return true
}

function ConnectScreen({ navigation }: {navigation: any}) {
  const [hasPermission, setHasPermission] = React.useState<boolean|null>(null);
  const [scanned, setScanned] = React.useState(false);

  React.useEffect(() => {
    (async () => {
        const { status } = await BarCodeScanner.requestPermissionsAsync();
        setHasPermission(status === "granted");
    })();
  }, []);

  const handleBarCodeScanned = ({ type, data } : {type: string, data: string}) => {
    if (validateQrCode(type, data)) {
      setScanned(true);
      navigation.navigate('ControllerScreen', {
        url: data
      });
    } else {
      alert(`INVALID CODE: ${type}, ${data}`);
    }
  };

  
  if (hasPermission === false) {
    // Will have to enter IP instead
    handleBarCodeScanned({
      type: "hi",
      data: "IP",
    });

    return (
      <Text>WIP</Text>
    );
  }
  

  return (
    <View style={styles.container}>
      {hasPermission && <BarCodeScanner onBarCodeScanned={scanned ? undefined : handleBarCodeScanned} style={StyleSheet.absoluteFillObject}/>}
      <TouchableWithoutFeedback onPress={() => {navigation.navigate('DisconnectModalScreen');}}>
        <View style={styles.enterCodeContainer}>
          <View style={styles.enterCodeTextView}>
            <View style={styles.enterCodeTextIconContainer}></View>
            <View style={styles.enterCodeTextIconContainer}>
              <Text style={styles.enterCodeText}>
                Enter Code
              </Text>
            </View>
            <View style={styles.enterCodeTextIconContainer}>
              <AntDesign name="rightcircleo" size={30} color="black" />
            </View>
          </View>
        </View>
      </TouchableWithoutFeedback>
    </View>
  );
}// <BarCodeScanner onBarCodeScanned={scanned ? undefined : handleBarCodeScanned} style={styles.camera}/>

function ConnectedScreen() {

}

function ControllerScreen({ route }: { route: any }) {

  const { url } = route.params;

  console.log(`url: ${url}`);

  
  var ws: WebSocket = new WebSocket(`${url}`);

  let data: any = [{Name: "NO OPEN PRESENTATIONS", HasSlideShowOpen: false, CurrentSlide: 0}];
  let currentPresentation: any = data[0];

  let isConnected: boolean = false;

  ws.onopen = () => {
    console.log("Connected");
    isConnected = true;
  }
  ws.onerror = (err) => {
    console.log(err);
  }
  ws.close = () => {
    isConnected = false;
    console.log("Disconnected");
  }
  ws.onmessage = (message) => {
    let messageJSON = JSON.parse(message.data);
    //console.log(messageJSON);

    if (messageJSON.presentations.length === 0) {
      data = [{Name: "NO OPEN PRESENTATIONS", HasSlideShowOpen: false, CurrentSlide: 0}];
    } else {
      data = messageJSON.presentations;
    }
    currentPresentation = data[0];

    console.log(data);
  }
  
  const PowerPointControl = {
    CheckConnection: function() {
      if (!isConnected) {
        console.log("IS NOT CONNECTED");
      }
    },
    Next: function() {
      this.CheckConnection();

      if (currentPresentation.Name != "NO OPEN PRESENTATION") {
        if (currentPresentation.HasSlideShowOpen) {
          ws.send(`{"presentation": "${currentPresentation.Name}", "command": "next"}`);
        }
      }
    },
  }

  return (
    <View style={styles.ControllerScreenContainer}>
      <View style={styles.ControllerScreenRows}>

      </View>
      <View style={styles.ControllerScreenRows}>
        <View style={styles.ControllerScreenTriSection}>
          <Button title="Previous" onPress={() => {console.log("PREVIOUS")}}/>
        </View>
        <View style={styles.ControllerScreenTriSection}>
          <Text>5</Text>
        </View>
        <View style={styles.ControllerScreenTriSection}>
          <Button title="Next" onPress={() => {PowerPointControl.Next()}}/>
        </View>
      </View>
      <View style={styles.ControllerScreenRows}>
        <View style={styles.ControllerScreenTriSection}>
          <Button title="Focus Slideshow" onPress={() => {console.log("FOCUS")}}/>
        </View>
        <View style={styles.ControllerScreenTriSection}>
          <Text>8</Text>
        </View>
        <View style={styles.ControllerScreenTriSection}>
          <Button title="Exit Slideshow" onPress={() => {console.log("EXIT")}}/>
        </View>
      </View>
    </View>
  );
}

function EnterCodeModalScreen() {
  return (
    <View style={styles.container}>
      <Text>EnterCodeModalScreen</Text>
    </View>
  );
}

function DisconnectModalScreen() {
  return (
    <View style={styles.container}>
      <Text>DisconnectModalScreen</Text>
    </View>
  );
}

export default function App() {
  return (
    <NavigationContainer>
      <Stack.Navigator initialRouteName="ConnectScreen">
        <Stack.Group screenOptions={{headerShown: false, gestureEnabled: false}}>
          <Stack.Screen name="ConnectScreen" component={ConnectScreen} />
          <Stack.Screen name="ControllerScreen" component={ControllerScreen} />
        </Stack.Group>
        <Stack.Group screenOptions={{presentation: 'modal', headerShown: false, gestureEnabled: true }}>
          <Stack.Screen name="EnterCodeModalScreen" component={EnterCodeModalScreen} />
          <Stack.Screen name="DisconnectModalScreen" component={DisconnectModalScreen} />
        </Stack.Group>
      </Stack.Navigator>
    </NavigationContainer>
  );
}

const _height = Dimensions.get("screen").height;
const _width  = Dimensions.get("screen").width;
const squareValue = Math.min(_height, _width) * 0.9;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#fff',
    alignItems: 'center',
    justifyContent: 'center',
  },
  enterCodeContainer: {
    height: squareValue * 0.2,
    width: squareValue,
    backgroundColor: "#d9d9d9",
    shadowColor: "#000",
    shadowOffset: {
      width: 0,
      height: 0,
    },
    shadowOpacity: 0.25,
    shadowRadius: 5,
    bottom: -250,
    borderRadius: 20,
  },
  enterCodeTextView: {
    margin: 10,
    flex: 1,
    flexDirection: 'row',
    justifyContent: "space-between",
    alignItems: 'center',
  },
  enterCodeTextIconContainer: {
    height: "100%",
    width: "33.33%",
    alignItems: 'center',
    justifyContent: 'center',
  },
  enterCodeText: {
    fontSize: 20,
  },
  camera: {
    height: "100%",
    width: "100%",
  },
  ControllerScreenContainer: {
    flex: 1,
    backgroundColor: 'green',
    alignItems: 'center',
    justifyContent: 'center',
    height: "100%",
    width: "100%",
  },
  ControllerScreenRows: {
    flex: 1,
    flexDirection: 'row',
    justifyContent: "space-between",
    alignItems: 'center',
    flexWrap: "nowrap",
    backgroundColor: "red",
    width: "100%",
  },
  ControllerScreenTriSection: {
    backgroundColor: "orange",
    flexGrow: 1,
    height: "100%",
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
  },
});
