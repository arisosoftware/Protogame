﻿

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Protogame
{
	public partial class NetworkSynchronisationComponent 
	{
		private void AssignSyncDataToMessage(List<SynchronisedData> dataList, EntityPropertiesMessage message, int frameTick, MxClientGroup endpoint, out bool mustBeReliable)
		{
			mustBeReliable = false;

			
			var totalString = 0;
			var currentString = 0;
			
			
			var totalInt16 = 0;
			var currentInt16 = 0;
			
			
			var totalInt32 = 0;
			var currentInt32 = 0;
			
			
			var totalSingle = 0;
			var currentSingle = 0;
			
			
			var totalDouble = 0;
			var currentDouble = 0;
			
			
			var totalBoolean = 0;
			var currentBoolean = 0;
			
			
			var totalSingleArray = 0;
			var currentSingleArray = 0;
			
			
			var totalTransform = 0;
			var currentTransform = 0;
			
						
			var typeLookup = new Dictionary<int, int>();
            for (var i = 0; i < dataList.Count; i++)
            {
				if (dataList[i].CurrentValue == null)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeNull;
					continue;
				}

								
				else if (dataList[i].CurrentValue is string)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeString;
					totalString += 1;
				}

								
				else if (dataList[i].CurrentValue is short)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeInt16;
					totalInt16 += 1;
				}

								
				else if (dataList[i].CurrentValue is int)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeInt32;
					totalInt32 += 1;
				}

								
				else if (dataList[i].CurrentValue is float)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeSingle;
					totalSingle += 1;
				}

								
				else if (dataList[i].CurrentValue is double)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeDouble;
					totalDouble += 1;
				}

								
				else if (dataList[i].CurrentValue is bool)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeBoolean;
					totalBoolean += 1;
				}

								
				else if (dataList[i].CurrentValue is Microsoft.Xna.Framework.Vector2)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeVector2;
					totalSingleArray += 2;
				}

								
				else if (dataList[i].CurrentValue is Microsoft.Xna.Framework.Vector3)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeVector3;
					totalSingleArray += 3;
				}

								
				else if (dataList[i].CurrentValue is Microsoft.Xna.Framework.Vector4)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeVector4;
					totalSingleArray += 4;
				}

								
				else if (dataList[i].CurrentValue is Microsoft.Xna.Framework.Quaternion)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeQuaternion;
					totalSingleArray += 4;
				}

								
				else if (dataList[i].CurrentValue is Microsoft.Xna.Framework.Matrix)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeMatrix;
					totalSingleArray += 16;
				}

								
				else if (dataList[i].CurrentValue is Protogame.NetworkTransform)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeTransform;
					totalTransform += 1;
				}

				
				else
				{
					throw new NotSupportedException("The type " + dataList[i].CurrentValue + " can not be synchronised as a network property.");
				}
			}

						if (totalString > 0)
			{
				message.PropertyValuesString = new string[totalString];
			}
						if (totalInt16 > 0)
			{
				message.PropertyValuesInt16 = new short[totalInt16];
			}
						if (totalInt32 > 0)
			{
				message.PropertyValuesInt32 = new int[totalInt32];
			}
						if (totalSingle > 0)
			{
				message.PropertyValuesSingle = new float[totalSingle];
			}
						if (totalDouble > 0)
			{
				message.PropertyValuesDouble = new double[totalDouble];
			}
						if (totalBoolean > 0)
			{
				message.PropertyValuesBoolean = new bool[totalBoolean];
			}
						if (totalSingleArray > 0)
			{
				message.PropertyValuesSingleArray = new float[totalSingleArray];
			}
						if (totalTransform > 0)
			{
				message.PropertyValuesTransform = new Protogame.NetworkTransform[totalTransform];
			}
			
            for (var ix = 0; ix < dataList.Count; ix++)
            {
				message.PropertyNames[ix] = dataList[ix].Name;
				message.PropertyTypes[ix] = typeLookup[ix];

				// Update synchronisation data.
				dataList[ix].LastFrameSynced[endpoint] = frameTick;

				if (!dataList[ix].HasPerformedInitialSync.GetOrDefault(endpoint))
				{
					dataList[ix].HasPerformedInitialSync[endpoint] = true;
					mustBeReliable = true;
				}

				object currentValue = dataList[ix].CurrentValue;
				switch (typeLookup[ix])
				{
					case EntityPropertiesMessage.PropertyTypeNull:
						// Do nothing.
						break;
									case EntityPropertiesMessage.PropertyTypeString:
					{
											string value = (string)currentValue;
										message.PropertyValuesString[currentString++] = value;
										}
						break;
									case EntityPropertiesMessage.PropertyTypeInt16:
					{
											short value = (short)currentValue;
										message.PropertyValuesInt16[currentInt16++] = value;
										}
						break;
									case EntityPropertiesMessage.PropertyTypeInt32:
					{
											int value = (int)currentValue;
										message.PropertyValuesInt32[currentInt32++] = value;
										}
						break;
									case EntityPropertiesMessage.PropertyTypeSingle:
					{
											float value = (float)currentValue;
										message.PropertyValuesSingle[currentSingle++] = value;
										}
						break;
									case EntityPropertiesMessage.PropertyTypeDouble:
					{
											double value = (double)currentValue;
										message.PropertyValuesDouble[currentDouble++] = value;
										}
						break;
									case EntityPropertiesMessage.PropertyTypeBoolean:
					{
											bool value = (bool)currentValue;
										message.PropertyValuesBoolean[currentBoolean++] = value;
										}
						break;
									case EntityPropertiesMessage.PropertyTypeVector2:
					{
											var value = ConvertToVector2(currentValue);
											message.PropertyValuesSingleArray[currentSingleArray++] = value[0];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[1];
										}
						break;
									case EntityPropertiesMessage.PropertyTypeVector3:
					{
											var value = ConvertToVector3(currentValue);
											message.PropertyValuesSingleArray[currentSingleArray++] = value[0];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[1];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[2];
										}
						break;
									case EntityPropertiesMessage.PropertyTypeVector4:
					{
											var value = ConvertToVector4(currentValue);
											message.PropertyValuesSingleArray[currentSingleArray++] = value[0];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[1];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[2];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[3];
										}
						break;
									case EntityPropertiesMessage.PropertyTypeQuaternion:
					{
											var value = ConvertToQuaternion(currentValue);
											message.PropertyValuesSingleArray[currentSingleArray++] = value[0];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[1];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[2];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[3];
										}
						break;
									case EntityPropertiesMessage.PropertyTypeMatrix:
					{
											var value = ConvertToMatrix(currentValue);
											message.PropertyValuesSingleArray[currentSingleArray++] = value[0];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[1];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[2];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[3];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[4];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[5];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[6];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[7];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[8];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[9];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[10];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[11];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[12];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[13];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[14];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[15];
										}
						break;
									case EntityPropertiesMessage.PropertyTypeTransform:
					{
											Protogame.NetworkTransform value = (Protogame.NetworkTransform)currentValue;
										message.PropertyValuesTransform[currentTransform++] = value;
										}
						break;
								}
			}
		}
		
		private void AssignMessageToSyncData(EntityPropertiesMessage message, Dictionary<string, SynchronisedData> fullDataList, MxClientGroup endpoint)
		{
			
			var currentString = 0;
			
			
			var currentInt16 = 0;
			
			
			var currentInt32 = 0;
			
			
			var currentSingle = 0;
			
			
			var currentDouble = 0;
			
			
			var currentBoolean = 0;
			
			
			var currentSingleArray = 0;
			
			
			var currentTransform = 0;
			
			
			for (var i = 0; i < message.PropertyNames.Length; i++)
			{
				if (!fullDataList.ContainsKey(message.PropertyNames[i]))
				{
					continue;
				}

				var syncData = fullDataList[message.PropertyNames[i]];
				var hasValue = false;
				object value = null;
				
				if (message.MessageOrder <= syncData.LastMessageOrder)
				{
					// This property is already at a later version.
					continue;
				}

				switch (message.PropertyTypes[i])
				{
					case EntityPropertiesMessage.PropertyTypeNone:
						break;
					case EntityPropertiesMessage.PropertyTypeNull:
						value = null;
						hasValue = true;
						syncData.HasReceivedInitialSync[endpoint] = true;
						break;
										case EntityPropertiesMessage.PropertyTypeString:
					{
											value = message.PropertyValuesString[currentString];
						hasValue = true;
						syncData.HasReceivedInitialSync[endpoint] = true;
						currentString++;
											break;
					}
										case EntityPropertiesMessage.PropertyTypeInt16:
					{
											value = message.PropertyValuesInt16[currentInt16];
						hasValue = true;
						syncData.HasReceivedInitialSync[endpoint] = true;
						currentInt16++;
											break;
					}
										case EntityPropertiesMessage.PropertyTypeInt32:
					{
											value = message.PropertyValuesInt32[currentInt32];
						hasValue = true;
						syncData.HasReceivedInitialSync[endpoint] = true;
						currentInt32++;
											break;
					}
										case EntityPropertiesMessage.PropertyTypeSingle:
					{
											value = message.PropertyValuesSingle[currentSingle];
						hasValue = true;
						syncData.HasReceivedInitialSync[endpoint] = true;
						currentSingle++;
											break;
					}
										case EntityPropertiesMessage.PropertyTypeDouble:
					{
											value = message.PropertyValuesDouble[currentDouble];
						hasValue = true;
						syncData.HasReceivedInitialSync[endpoint] = true;
						currentDouble++;
											break;
					}
										case EntityPropertiesMessage.PropertyTypeBoolean:
					{
											value = message.PropertyValuesBoolean[currentBoolean];
						hasValue = true;
						syncData.HasReceivedInitialSync[endpoint] = true;
						currentBoolean++;
											break;
					}
										case EntityPropertiesMessage.PropertyTypeVector2:
					{
											value = ConvertFromVector2(message.PropertyValuesSingleArray, currentSingleArray);
						hasValue = true;
						syncData.HasReceivedInitialSync[endpoint] = true;
						currentSingleArray += 2;
											break;
					}
										case EntityPropertiesMessage.PropertyTypeVector3:
					{
											value = ConvertFromVector3(message.PropertyValuesSingleArray, currentSingleArray);
						hasValue = true;
						syncData.HasReceivedInitialSync[endpoint] = true;
						currentSingleArray += 3;
											break;
					}
										case EntityPropertiesMessage.PropertyTypeVector4:
					{
											value = ConvertFromVector4(message.PropertyValuesSingleArray, currentSingleArray);
						hasValue = true;
						syncData.HasReceivedInitialSync[endpoint] = true;
						currentSingleArray += 4;
											break;
					}
										case EntityPropertiesMessage.PropertyTypeQuaternion:
					{
											value = ConvertFromQuaternion(message.PropertyValuesSingleArray, currentSingleArray);
						hasValue = true;
						syncData.HasReceivedInitialSync[endpoint] = true;
						currentSingleArray += 4;
											break;
					}
										case EntityPropertiesMessage.PropertyTypeMatrix:
					{
											value = ConvertFromMatrix(message.PropertyValuesSingleArray, currentSingleArray);
						hasValue = true;
						syncData.HasReceivedInitialSync[endpoint] = true;
						currentSingleArray += 16;
											break;
					}
										case EntityPropertiesMessage.PropertyTypeTransform:
					{
											value = message.PropertyValuesTransform[currentTransform];
						hasValue = true;
						syncData.HasReceivedInitialSync[endpoint] = true;
						currentTransform++;
											break;
					}
									}

				if (hasValue)
				{
					syncData.LastValueFromServer = value;
					syncData.LastMessageOrder = message.MessageOrder;

					if (syncData.TimeMachine == null)
					{
						syncData.SetValueDelegate(value);
					}
					else
					{
						syncData.TimeMachine.Set(message.FrameTick, value);
					}
				}
			}
		}
	}
}

