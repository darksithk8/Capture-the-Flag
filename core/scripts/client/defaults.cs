//-----------------------------------------------------------------------------
// Torque Game Engine Advanced
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

// The master server is declared with the server defaults, which is
// loaded on both clients & dedicated servers.  If the server mod
// is not loaded on a client, then the master must be defined. 
// $pref::Master[0] = "2:master.garagegames.com:28002";

$pref::Player::Name = "Visitor";
$pref::Player::defaultFov = 90;
$pref::Player::zoomSpeed = 0;

$pref::Net::LagThreshold = 400;
$pref::Net::Port = 28000;

$pref::shadows = "2";
$pref::HudMessageLogSize = 40;
$pref::ChatHudLength = 1;

$pref::Input::LinkMouseSensitivity = 1;
// DInput keyboard, mouse, and joystick prefs
$pref::Input::KeyboardEnabled = 1;
$pref::Input::MouseEnabled = 1;
$pref::Input::JoystickEnabled = 0;
$pref::Input::KeyboardTurnSpeed = 0.1;

$pref::sceneLighting::cacheSize = 20000;
$pref::sceneLighting::purgeMethod = "lastCreated";
$pref::sceneLighting::cacheLighting = 1;
$pref::sceneLighting::terrainGenerateLevel = 1;

$pref::ts::detailAdjust = 1;

$pref::Terrain::DynamicLights = 1;
$pref::Interior::TexturedFog = 0;

$Pref::GuiEditor::PreviewResolution = "1024 768";

$pref::Video::displayDevice = "D3D9";
$pref::Video::appliedPref = 0;
$pref::Video::disableVerticalSync = 1;
$pref::Video::monitorNum = 0;
$pref::Video::mode = "1024 768 false 32 60 0";
$pref::Video::windowedRes = "1024 768";
$pref::Video::Resolution = "1024 768";
$pref::Video::fullScreen = 0;
$pref::Video::BitsPerPixel = "32";
$pref::Video::RefreshRate = "60";
$pref::Video::FSAALevel = "0";
$pref::Video::MinBitsPerPixel = "32";
$pref::Video::defaultFenceCount = 0;
$pref::Video::screenShotSession = 0;
$pref::Video::screenShotFormat = "PNG";
$pref::video::sfxBackBufferSize = "512";

// Console-friendly defaults
if($platform $= "xenon")
{
   $pref::Video::mode = "1280 720 true 32 60 0";
   $pref::Video::Resolution = "1280 720";
   $pref::Video::fullScreen = 1;
}

/// This is the path used by ShaderGen to cache procedural
/// shaders.  If left blank ShaderGen will only cache shaders
/// to memory and not to disk.
$pref::video::shaderGenPath = "shaders/procedural";

/// The perfered light manager to use at startup.  If blank
/// or if the selected one doesn't work on this platfom it
/// will try the defaults below.
$pref::lightManager = "";

/// This is the default list of light managers ordered from
/// most to least desirable for initialization.
$pref::defaultLightManagers = "Advanced Lighting" NL "Basic Lighting";

/// A scale to apply to the normal visible distance
/// typically used for tuning performance.
$pref::visibleDistanceMod = 1.0;

/// Causes the system to do a one time autodetect
/// of an SFX provider and device at startup if the
/// provider is unset.
$pref::SFX::autoDetect = true;

/// The sound provider to select at startup.  Typically
/// this is DirectSound, OpenAL, or XACT.  There is also 
/// a special Null provider which acts normally, but 
/// plays no sound.
$pref::SFX::provider = "";

/// The sound device to select from the provider.  Each
/// provider may have several different devices.
$pref::SFX::device = "";

/// If true the device will try to use hardware buffers
/// and sound mixing.  If not it will use software.
$pref::SFX::useHardware = false;

/// If you have a software device you have a 
/// choice of how many software buffers to
/// allow at any one time.  More buffers cost
/// more CPU time to process and mix.
$pref::SFX::maxSoftwareBuffers = 16;

/// This is the playback frequency for the primary 
/// sound buffer used for mixing.  Although most
/// providers will reformat on the fly, for best 
/// quality and performance match your sound files
/// to this setting.
$pref::SFX::frequency = 44100;

/// This is the playback bitrate for the primary 
/// sound buffer used for mixing.  Although most
/// providers will reformat on the fly, for best 
/// quality and performance match your sound files
/// to this setting.
$pref::SFX::bitrate = 32;

/// The overall system volume at startup.  Note that 
/// you can only scale volume down, volume does not
/// get louder than 1.
$pref::SFX::masterVolume = 0.8;

/// The startup sound channel volumes.  These are 
/// used to control the overall volume of different 
/// classes of sounds.
$pref::SFX::channelVolume1 = 1;
$pref::SFX::channelVolume2 = 1;
$pref::SFX::channelVolume3 = 1;
$pref::SFX::channelVolume4 = 1;
$pref::SFX::channelVolume5 = 1;
$pref::SFX::channelVolume6 = 1;
$pref::SFX::channelVolume7 = 1;
$pref::SFX::channelVolume8 = 1;

/// These are the default PostFX settings
$pref::PostEffect::SSAO = false;
$pref::PostEffect::EdgeAA = false;
$pref::PostEffect::LightRay = false;
$pref::PostEffect::HDR = false;