﻿using Imaging.DDSReader.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Imaging.Tests.DDSReaderTest
{
	[TestClass]
	public class HelperTest
	{
		[TestMethod]
		public void TestComputeMaskParams_Mask_0()
		{
			var result = TestHelper.ComputeMaskParams(0);
			CollectionAssert.AreEqual(new int[] { 0, 1, 0 }, result);
		}

		[TestMethod]
		public void TestComputeMaskParams_Mask_1()
		{
			var result = TestHelper.ComputeMaskParams(1);
			CollectionAssert.AreEqual(new int[] { 0, 255, 0 }, result);
		}

		[TestMethod]
		public void TestComputeMaskParams_Mask_255()
		{
			var result = TestHelper.ComputeMaskParams(255);
			CollectionAssert.AreEqual(new int[] { 0, 1, 0 }, result);
		}

		[TestMethod]
		public void TestComputeMaskParams_Mask_Max()
		{
			var result = TestHelper.ComputeMaskParams(uint.MaxValue);
			CollectionAssert.AreEqual(new int[] { 0, 1, 0 }, result);
		}
	}
}