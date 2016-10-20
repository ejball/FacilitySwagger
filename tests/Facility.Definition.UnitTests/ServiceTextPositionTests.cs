﻿using Xunit;

namespace Facility.Definition.UnitTests
{
	public class ServiceMethodInfoTests
	{
		[Fact]
		public void InvalidNameThrows()
		{
			var position = new ServiceTextPosition("source");
			TestUtility.ThrowsServiceDefinitionException(() => new ServiceMethodInfo(name: "4u", position: position), position);
		}

		[Theory, InlineData(true), InlineData(false)]
		public void DuplicateFieldThrows(bool isRequest)
		{
			var fields = new[]
			{
				new ServiceFieldInfo("why", "int32", position: new ServiceTextPosition("source", 1)),
				new ServiceFieldInfo("Why", "int32", position: new ServiceTextPosition("source", 2)),
			};
			TestUtility.ThrowsServiceDefinitionException(
				() => new ServiceMethodInfo(name: "x", requestFields: isRequest ? fields : null, responseFields: isRequest ? null : fields), fields[1].Position);
		}
	}
}