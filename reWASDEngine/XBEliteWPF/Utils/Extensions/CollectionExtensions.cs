using System;
using System.Collections.Generic;
using System.Linq;
using DiscSoft.NET.Common.Utils;
using DiscSoftReWASDServiceNamespace;
using XBEliteWPF.DataModels.GamepadActiveProfiles;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.Controller;

namespace XBEliteWPF.Utils.Extensions
{
	public static class CollectionExtensions
	{
		public static REWASD_CONTROLLER_PROFILE_EX FindByID(this IEnumerable<REWASD_CONTROLLER_PROFILE_EX> collection, string ID)
		{
			if (string.IsNullOrEmpty(ID))
			{
				return null;
			}
			Func<ulong, bool> <>9__2;
			Func<REWASD_CONTROLLER_PROFILE, bool> <>9__1;
			return collection.FirstOrDefault(delegate(REWASD_CONTROLLER_PROFILE_EX pex)
			{
				IEnumerable<REWASD_CONTROLLER_PROFILE> profiles = pex.Profiles;
				Func<REWASD_CONTROLLER_PROFILE, bool> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = delegate(REWASD_CONTROLLER_PROFILE p)
					{
						IEnumerable<ulong> id2 = p.Id;
						Func<ulong, bool> func2;
						if ((func2 = <>9__2) == null)
						{
							func2 = (<>9__2 = (ulong id) => id != 0UL && ID.Contains(id.ToString()));
						}
						return id2.Any(func2);
					});
				}
				return profiles.Any(func);
			});
		}

		public static REWASD_CONTROLLER_PROFILE_EX FindByID(this IEnumerable<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> collection, string ID)
		{
			Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper = collection.FindWrapperByID(ID);
			if (wrapper == null)
			{
				return null;
			}
			return wrapper.Value;
		}

		public static Wrapper<REWASD_CONTROLLER_PROFILE_EX> FindWrapperByID(this IEnumerable<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> collection, string ID)
		{
			if (string.IsNullOrEmpty(ID))
			{
				return null;
			}
			Func<ulong, bool> <>9__2;
			Func<REWASD_CONTROLLER_PROFILE, bool> <>9__1;
			return collection.FirstOrDefault(delegate(Wrapper<REWASD_CONTROLLER_PROFILE_EX> pex)
			{
				IEnumerable<REWASD_CONTROLLER_PROFILE> profiles = pex.Value.Profiles;
				Func<REWASD_CONTROLLER_PROFILE, bool> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = delegate(REWASD_CONTROLLER_PROFILE p)
					{
						IEnumerable<ulong> id2 = p.Id;
						Func<ulong, bool> func2;
						if ((func2 = <>9__2) == null)
						{
							func2 = (<>9__2 = (ulong id) => id != 0UL && ID.Contains(id.ToString()));
						}
						return id2.Any(func2);
					});
				}
				return profiles.Any(func);
			});
		}

		public static REWASD_CONTROLLER_PROFILE_EX FindByAmiiboUid(this IEnumerable<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> collection, byte[] amiiboUid)
		{
			Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper = collection.FindWrapperByAmiiboUid(amiiboUid);
			if (wrapper == null)
			{
				return null;
			}
			return wrapper.Value;
		}

		public static Wrapper<REWASD_CONTROLLER_PROFILE_EX> FindWrapperByAmiiboUid(this IEnumerable<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> collection, byte[] amiiboUid)
		{
			if (amiiboUid == null || amiiboUid.Length != 7)
			{
				return null;
			}
			Func<REWASD_CONTROLLER_PROFILE_EX.AmiiboWrapper, bool> <>9__1;
			return collection.FirstOrDefault(delegate(Wrapper<REWASD_CONTROLLER_PROFILE_EX> pex)
			{
				IEnumerable<REWASD_CONTROLLER_PROFILE_EX.AmiiboWrapper> amiibo = pex.Value.Amiibo;
				Func<REWASD_CONTROLLER_PROFILE_EX.AmiiboWrapper, bool> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = (REWASD_CONTROLLER_PROFILE_EX.AmiiboWrapper p) => p.IsUidEqual(amiiboUid));
				}
				return amiibo.Any(func);
			});
		}

		public static REWASD_CONTROLLER_PROFILE_EX FindByControllerID(this IEnumerable<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> collection, ulong controllerId)
		{
			Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper = collection.FindWrapperByControllerID(controllerId);
			if (wrapper == null)
			{
				return null;
			}
			return wrapper.Value;
		}

		public static Wrapper<REWASD_CONTROLLER_PROFILE_EX> FindWrapperByControllerID(this IEnumerable<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> collection, ulong controllerId)
		{
			if (controllerId == 0UL)
			{
				return null;
			}
			Func<ulong, bool> <>9__2;
			Func<REWASD_CONTROLLER_PROFILE, bool> <>9__1;
			return collection.FirstOrDefault(delegate(Wrapper<REWASD_CONTROLLER_PROFILE_EX> pex)
			{
				IEnumerable<REWASD_CONTROLLER_PROFILE> profiles = pex.Value.Profiles;
				Func<REWASD_CONTROLLER_PROFILE, bool> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = delegate(REWASD_CONTROLLER_PROFILE p)
					{
						IEnumerable<ulong> id2 = p.Id;
						Func<ulong, bool> func2;
						if ((func2 = <>9__2) == null)
						{
							func2 = (<>9__2 = (ulong id) => id != 0UL && id == controllerId);
						}
						return id2.Any(func2);
					});
				}
				return profiles.Any(func);
			});
		}

		public static Wrapper<REWASD_CONTROLLER_PROFILE_EX> FindWrapperByServiceProfileId(this IEnumerable<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> collection, ulong serviceProfileId)
		{
			if (serviceProfileId == 0UL)
			{
				return null;
			}
			Func<ushort, bool> <>9__1;
			return collection.FirstOrDefault(delegate(Wrapper<REWASD_CONTROLLER_PROFILE_EX> pex)
			{
				IEnumerable<ushort> serviceProfileIds = pex.Value.ServiceProfileIds;
				Func<ushort, bool> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = (ushort p) => (ulong)p == serviceProfileId);
				}
				return serviceProfileIds.Any(func);
			});
		}

		public static List<REWASD_CONTROLLER_PROFILE> FindServiceProfilesByExternalMacAddress(this IEnumerable<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> collection, ulong macAddress)
		{
			List<REWASD_CONTROLLER_PROFILE> list = new List<REWASD_CONTROLLER_PROFILE>();
			if (macAddress == 0UL)
			{
				return list;
			}
			Func<REWASD_CONTROLLER_PROFILE, bool> <>9__1;
			Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper = collection.FirstOrDefault(delegate(Wrapper<REWASD_CONTROLLER_PROFILE_EX> pex)
			{
				IEnumerable<REWASD_CONTROLLER_PROFILE> profiles2 = pex.Value.Profiles;
				Func<REWASD_CONTROLLER_PROFILE, bool> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = (REWASD_CONTROLLER_PROFILE p) => p.RemoteBthAddr == macAddress);
				}
				return profiles2.Any(func);
			});
			if (((wrapper != null) ? wrapper.Value : null) != null)
			{
				foreach (REWASD_CONTROLLER_PROFILE rewasd_CONTROLLER_PROFILE in wrapper.Value.Profiles)
				{
					if (rewasd_CONTROLLER_PROFILE.RemoteBthAddr == macAddress)
					{
						list.Add(rewasd_CONTROLLER_PROFILE);
					}
				}
			}
			return list;
		}

		public static bool ContainsProfileForID(this IEnumerable<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> collection, string ID)
		{
			return collection.FindByID(ID) != null;
		}

		public static bool ContainsControllerInfoForControllerId(this IEnumerable<REWASD_CONTROLLER_INFO> collection, ulong controllerId)
		{
			return collection.Any((REWASD_CONTROLLER_INFO rci) => rci.Id == controllerId);
		}

		public static REWASD_CONTROLLER_INFO? FindControllerInfoForControllerId(this IEnumerable<REWASD_CONTROLLER_INFO> collection, ulong controllerId)
		{
			return new REWASD_CONTROLLER_INFO?(collection.FirstOrDefault((REWASD_CONTROLLER_INFO rci) => rci.Id == controllerId));
		}

		public static BaseControllerVM FindControllerByControllerId(this IEnumerable<BaseControllerVM> collection, ulong controllerId)
		{
			return collection.FirstOrDefault((BaseControllerVM c) => c.ID.Contains(controllerId.ToString()));
		}

		public static GamepadProfiles FindProfileByControllerId(this ObservableDictionary<string, GamepadProfiles> collection, string controllerId)
		{
			if (string.IsNullOrEmpty(controllerId))
			{
				return null;
			}
			return collection.FirstOrDefault((KeyValuePair<string, GamepadProfiles> kvp) => kvp.Key.Contains(controllerId)).Value;
		}
	}
}
